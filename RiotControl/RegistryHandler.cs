using Microsoft.Win32;
using System;
using System.Reflection;

namespace RiotControl
{
	static class RegistryHandler
	{
		const string RegistryRunPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

		const string RiotControlKey = "Riot Control";

		static bool IsAutorun(string keyName, string path)
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryRunPath);
			if (key == null)
				return false;

			string currentPath = (string)key.GetValue(keyName);
			if (currentPath == null)
				return false;

			return currentPath == path;
		}

		static void EnableAutorun(string keyName, string path)
		{
			RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryRunPath);
			key.SetValue(keyName, path);
		}

		static void DisableAutorun(string keyName)
		{
			try
			{
				RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryRunPath, true);
				key.DeleteValue(keyName);
			}
			catch (ArgumentException)
			{
			}
		}

		static string GetApplicationPath()
		{
			string path = Assembly.GetEntryAssembly().Location;
			return string.Format("\"{0}\"", path);
		}

		public static bool IsAutorun()
		{
			return IsAutorun(RiotControlKey, GetApplicationPath());
		}

		public static void EnableAutorun()
		{
			EnableAutorun(RiotControlKey, GetApplicationPath());
		}

		public static void DisableAutorun()
		{
			DisableAutorun(RiotControlKey);
		}
	}
}
