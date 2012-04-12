using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace RiotUpdate
{
	class Program
	{
		static void WaitForProcessTermination(string application)
		{
			while (true)
			{
				var processes = Process.GetProcessesByName(application);
				if (processes.Length == 0)
					return;
				Thread.Sleep(0);
			}
		}

		static List<Regex> GetFilters(string patternStrings)
		{
			var tokens = patternStrings.Split(';');
			return (from x in tokens select new Regex(x)).ToList();
		}

		static void ApplyUpdate(string updateDirectory, List<Regex> filters)
		{
			DirectoryInfo directory = new DirectoryInfo(updateDirectory);
			foreach (var file in directory.GetFiles())
			{
				bool isAMatch = false;
				foreach (var filter in filters)
				{
					var match = filter.Match(file.Name);
					if (match.Success)
					{
						isAMatch = true;
						break;
					}
				}
				if (isAMatch)
				{
					Console.WriteLine("Updating {0}", file.Name);
					Exception lastException = null;
					for (int i = 0; i < 10; i++)
					{
						try
						{
							File.Delete(file.Name);
							File.Copy(file.FullName, file.Name);
							File.Delete(file.FullName);
							lastException = null;
							break;
						}
						catch (Exception exception)
						{
							lastException = exception;
							Thread.Sleep(300);
						}
					}
					if (lastException != null)
						throw lastException;
				}
				else
					Console.WriteLine("Ignoring {0}", file.Name);
			}
		}

		static void RunApplication(string application)
		{
			Console.WriteLine("Running {0}", application);
			Process.Start(application);
		}

		static void Main(string[] arguments)
		{
			if (arguments.Length < 2)
			{
				Console.WriteLine("Usage:");
				Console.WriteLine("<update directory> <list of regular expressions that match the names of files that need to be updated, separated by semicolons> <optional: application to launch after update>");
				Console.ReadLine();
				return;
			}

			string updateDirectory = arguments[0];
			string patternStrings = arguments[1];
			string application;
			if (arguments.Length >= 3)
				application = arguments[2];
			else
				application = null;

			Console.WriteLine("Applying update");

			Console.WriteLine("Update directory: {0}", updateDirectory);
			Console.WriteLine("Pattern strings: {0}", patternStrings);
			Console.WriteLine("Application to launch: {0}", application);

			try
			{
				WaitForProcessTermination(application);
				var filters = GetFilters(patternStrings);
				ApplyUpdate(updateDirectory, filters);
				if(application != null)
					RunApplication(application);
			}
			catch (Exception exception)
			{
				Console.WriteLine("Update failed: {0}", exception.Message);
				bool IsMono = Type.GetType("Mono.Runtime") != null;
				if (!IsMono)
				{
					//For those pesky Windows users...
					Console.ReadLine();
				}
				return;
			}

			Console.WriteLine("Update succeeded");
		}
	}
}
