using System;
using System.Configuration;
using System.Data.Common;

namespace RiotGear
{
	public class Database
	{
		DbProviderFactory Factory;
		string Path;

		public Database(Configuration configuration)
		{
			string provider = configuration.DatabaseProvider;
			try
			{
				Factory = DbProviderFactories.GetFactory(provider);
				Path = configuration.Database;
			}
			catch (ConfigurationException exception)
			{
				throw new Exception(string.Format("Unable to load database provider {0}: {1}", provider, exception.Message));
			}
		}

		public DbConnection GetConnection()
		{
			DbConnection connection = Factory.CreateConnection();
			connection.ConnectionString = string.Format("Data Source = {0}", Path);
			connection.Open();
			//Turn on SQLite foreign keys
			using (var pragma = new DatabaseCommand("pragma foreign_keys = on", connection))
			{
				pragma.Execute();
			}
			return connection;
		}
	}
}
