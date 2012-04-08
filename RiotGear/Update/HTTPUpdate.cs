using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace RiotGear
{
	public class HTTPUpdate
	{
		const bool ForceUpdate = true;
		const string UpdateDirectory = "Update";

		UpdateConfiguration Configuration;

		IGlobalHandler GlobalHandler;
		IUpdateHandler UpdateHandler;

		int CurrentRevision;
		ApplicationVersion NewestVersion;

		public HTTPUpdate(Configuration configuration, IGlobalHandler globalHandler, IUpdateHandler updateHandler)
		{
			Configuration = configuration.Updates;

			GlobalHandler = globalHandler;
			UpdateHandler = updateHandler;

			CurrentRevision = Assembly.GetEntryAssembly().GetName().Version.Revision;
			NewestVersion = null;
		}

		public void Run()
		{
			Thread updateThread = new Thread(CheckForUpdate);
			updateThread.Name = "HTTP update thread";
			updateThread.Start();
		}

		void WriteLine(string line, params object[] arguments)
		{
			string message = string.Format("[Update check] {0}", line);
			GlobalHandler.WriteLine(message, arguments);
		}

		void CheckForUpdate()
		{
			try
			{
				WebClient client = new WebClient();
				List<ApplicationVersion> versions = new List<ApplicationVersion>();
				string index = client.DownloadString(Configuration.UpdateURL);
				Regex pattern = new Regex(Configuration.ReleasePattern);
				foreach (Match match in pattern.Matches(index))
				{
					string filename = match.Value;
					string revisionString = match.Groups[1].Value;
					int revision = Convert.ToInt32(revisionString);
					ApplicationVersion version = new ApplicationVersion(filename, revision);
					versions.Add(version);
				}
				if (versions.Count == 0)
				{
					WriteLine("No versions of this application are being offered by the update server - this should never happen.");
					return;
				}
				//Sort the archives by revision to easily determine the newest one
				versions.Sort();
				NewestVersion = versions[0];
				int newestRevision = NewestVersion.Revision;
				if (CurrentRevision < newestRevision || ForceUpdate)
				{
					WriteLine("The current version of this software (r{0}) is outdated. The newest version available is r{1}.", CurrentRevision, newestRevision);
					UpdateHandler.UpdateDetected(CurrentRevision, NewestVersion);
				}
				else if (CurrentRevision == newestRevision)
					WriteLine("This software is up to date.");
				else
					WriteLine("The current version (r{0}) is newer than the most recent version of this software available (r{1}), how odd.", CurrentRevision, newestRevision);
			}
			catch (ArgumentException exception)
			{
				WriteLine("Invalid update release pattern specified: {0}", exception.Message);
			}
			catch (WebException exception)
			{
				WriteLine("Unable to determine the latest version of this software: {0}", exception.Message);
			}
		}

		string GetDownloadPath()
		{
			return Path.Combine(UpdateDirectory, NewestVersion.Filename);
		}

		public void StartUpdateDownload()
		{
			try
			{
				string downloadURL = Configuration.UpdateURL + NewestVersion.Filename;
				string downloadPath = GetDownloadPath();

				Directory.CreateDirectory(UpdateDirectory);

				WebClient client = new WebClient();
				client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
				client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
				client.DownloadFileAsync(new Uri(downloadURL), downloadPath);
				WriteLine("Downloading {0} to {1}", downloadURL, downloadPath);
			}
			catch (Exception exception)
			{
				WriteLine("Unable to download the latest version of this software: {0}", exception.Message);
			}
		}

		void UnzipFile(string archivePath)
		{
			using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(archivePath)))
			{
				ZipEntry zipEntry;
				while ((zipEntry = zipStream.GetNextEntry()) != null)
				{
					string directoryName = Path.Combine(UpdateDirectory, Path.GetDirectoryName(zipEntry.Name));
					if (directoryName.Length > 0 && !Directory.Exists(directoryName))
					{
						//Create the directory
						Directory.CreateDirectory(directoryName);
						WriteLine("Created directory {0}", directoryName);
					}

					string fileName = Path.GetFileName(zipEntry.Name);
					if (fileName != string.Empty)
					{
						//Unpack the file
						using (FileStream streamWriter = File.Create(Path.Combine(UpdateDirectory, zipEntry.Name)))
						{
							int chunkSize = 2048;
							byte[] data = new byte[chunkSize];
							while (true)
							{
								chunkSize = zipStream.Read(data, 0, data.Length);
								if (chunkSize > 0)
									streamWriter.Write(data, 0, chunkSize);
								else
									break;
							}
						}
						WriteLine("Unpacked file {0}", zipEntry.Name);
					}
				}
			}
		}

		void DownloadFileCompleted(object sender, AsyncCompletedEventArgs arguments)
		{
			if (arguments.Error == null)
			{
				WriteLine("Download of {0} completed", NewestVersion.Filename);

				string archivePath = GetDownloadPath();
				UnzipFile(archivePath);
				File.Delete(archivePath);

				WriteLine("Unpacked archive {0} to {1}", NewestVersion.Filename, UpdateDirectory);

				UpdateHandler.DownloadCompleted();
			}
			else
			{
				WriteLine("A download error occurred: {0}", arguments.Error.Message);

				UpdateHandler.DownloadError(arguments.Error);
			}
		}

		void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs arguments)
		{
			UpdateHandler.DownloadProgressUpdate(arguments);
		}
	}
}
