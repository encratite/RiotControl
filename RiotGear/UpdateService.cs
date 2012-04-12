using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	public class UpdateService
	{
		public const string UpdateDirectory = "Update";
		public const string UpdateApplication = "Update.exe";

		UpdateConfiguration Configuration;

		IGlobalHandler GlobalHandler;
		IUpdateHandler UpdateHandler;

		int CurrentRevision;
		ApplicationVersion NewestVersion;

		bool IsCommandLineVersion;
		bool IsMono;

		public UpdateService(Configuration configuration, IGlobalHandler globalHandler, IUpdateHandler updateHandler = null)
		{
			Configuration = configuration.Updates;

			GlobalHandler = globalHandler;
			UpdateHandler = updateHandler;

			IsCommandLineVersion = UpdateHandler == null;
			IsMono = Type.GetType("Mono.Runtime") != null;

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

		public void Cleanup()
		{
			if(Directory.Exists(UpdateDirectory))
			{
				try
				{
					string source = Path.Combine(UpdateDirectory, UpdateApplication);
					File.Delete(UpdateApplication);
					File.Copy(source, UpdateApplication);
					WriteLine("Replaced {0}", UpdateApplication);
				}
				catch (Exception exception)
				{
					WriteLine("Failed to replace {0}: {1}", UpdateApplication, exception.Message);
				}
				Directory.Delete(UpdateDirectory, true);
				WriteLine("Removed the update directory");
			}
		}

		public void CheckForUpdate()
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
				if (CurrentRevision < newestRevision || Configuration.ForceUpdate)
				{
					WriteLine("The current version of this software (r{0}) is outdated. The newest version available is r{1}.", CurrentRevision, newestRevision);
					if (UpdateHandler != null)
						UpdateHandler.UpdateDetected(CurrentRevision, NewestVersion);
					else
						DownloadUpdate(false);
				}
				else if (CurrentRevision == newestRevision)
					WriteLine("This software is up to date.");
				else
					WriteLine("The current version (r{0}) is newer than the most recent version of this software available (r{1}).", CurrentRevision, newestRevision);
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

		public void ApplyUpdate()
		{
			string patternString = string.Join(";", Configuration.UpdateTargets);
			var name = Assembly.GetEntryAssembly().GetName();
			string application = string.Format("{0}.exe", name.Name);
			string arguments = string.Format("\"{0}\" \"{1}\"", UpdateService.UpdateDirectory, patternString);
			if (!IsCommandLineVersion)
			{
				//Only restart the application automatically if it's not the CLI version
				arguments = string.Format("{0} \"{1}\"", arguments, application);
			}
			if (IsMono)
			{
				ProcessStartInfo information = new ProcessStartInfo("mono", string.Format("{0} {1}", UpdateApplication, arguments));
				information.UseShellExecute = true;
				Process updateProcess = Process.Start(information);
				updateProcess.WaitForExit();
				Environment.Exit(0);
			}
			else
			{
				Process.Start(UpdateApplication, arguments);
				Process.GetCurrentProcess().Kill();
			}
		}

		string GetDownloadPath()
		{
			return Path.Combine(UpdateDirectory, NewestVersion.Filename);
		}

		public void DownloadUpdate(bool asynchronousDownload = true)
		{
			try
			{
				string downloadURL = Configuration.UpdateURL + NewestVersion.Filename;
				string downloadPath = GetDownloadPath();

				Directory.CreateDirectory(UpdateDirectory);

				WebClient client = new WebClient();
				var uri = new Uri(downloadURL);
				WriteLine("Downloading {0} to {1}", downloadURL, downloadPath);
				if (asynchronousDownload)
				{
					client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
					client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
					client.DownloadFileAsync(uri, downloadPath);
				}
				else
				{
					client.DownloadFile(uri, downloadPath);
					WriteLine("Downloaded new version to {0}", downloadPath);
					ProcessArchive();
					ApplyUpdate();
				}
			}
			catch (Exception exception)
			{
				string message = string.Format("Unable to download the latest version of this software: {0}", exception.Message);
				if (asynchronousDownload)
					WriteLine(message);
				else
					throw new Exception(message);
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

		void ProcessArchive()
		{
			string archivePath = GetDownloadPath();
			string extension = Path.GetExtension(archivePath);
			if (extension == ".bz2")
			{
				//We're probably in a UNIX-like environment
				string archiveName = NewestVersion.Filename;
				int offset = archiveName.IndexOf('.');
				if (offset == -1)
					throw new Exception("Unable to parse archive name to determine base name");
				string baseName = archiveName.Substring(0, offset);
				Process process = new Process();
				process.StartInfo.FileName = "tar";
				process.StartInfo.Arguments = string.Format("-C {0} -xf {1}", UpdateDirectory, archivePath);
				process.Start();
				process.WaitForExit();
				if(process.ExitCode != 0)
					throw new Exception("Failed to unpack archive");
				//Move the inner directory to the top level so it looks just like the directory structure used in the Windows releases
				//This has to be done because the .tar.bz2 uses a different directory structure with an additional top-level entry
				const string temporaryDirectory = "Temporary";
				Directory.Move(UpdateDirectory, temporaryDirectory);
				Directory.Move(Path.Combine(temporaryDirectory, baseName), UpdateDirectory);
				Directory.Delete(temporaryDirectory, true);
			}
			else if(extension == ".zip")
			{
				UnzipFile(archivePath);
				File.Delete(archivePath);
			}
			else
				throw new Exception("Unknown file extension, cannot proceed with update");
		}

		void DownloadFileCompleted(object sender, AsyncCompletedEventArgs arguments)
		{
			if (arguments.Error == null)
			{
				WriteLine("Download of {0} completed", NewestVersion.Filename);

				ProcessArchive();

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
