using System;
using System.Configuration;
using System.Data.Common;

namespace RiotGear
{
	public class Database
	{
		DbProviderFactory Factory;
		Configuration Configuration;

		public Database(Configuration configuration)
		{
			Configuration = configuration;
			try
			{
				Factory = DbProviderFactories.GetFactory(Configuration.DatabaseProvider);
			}
			catch (ConfigurationException exception)
			{
				throw new Exception(string.Format("Unable to load database provider {0}: {1}", Configuration.DatabaseProvider, exception.Message));
			}
		}

		public DbConnection GetConnection()
		{
			DbConnection connection = Factory.CreateConnection();
			connection.ConnectionString = Configuration.Database;
			connection.Open();
			//Turn on SQLite foreign keys
			if (IsSQLite())
			{
				using (var pragma = new DatabaseCommand("pragma foreign_keys = on", connection))
				{
					pragma.Execute();
				}
			}
			return connection;
		}

		public bool IsSQLite()
		{
			return Configuration.DatabaseProvider == "System.Data.SQLite" || Configuration.DatabaseProvider == "Mono.Data.Sqlite";
		}
	}
}
