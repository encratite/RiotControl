using System;
using System.Configuration;
using System.Data.Common;

namespace RiotGear
{
	public enum DatabaseType
	{
		SQLite,
		PostgreSQL,
		Unknown,
	}

	public class Database
	{
		DbProviderFactory Factory;
		Configuration Configuration;

		public readonly DatabaseType Type;

		public Database(Configuration configuration)
		{
			Configuration = configuration;
			Type = GetType();
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

		DatabaseType GetType()
		{
			if (Configuration.DatabaseProvider == "System.Data.SQLite" || Configuration.DatabaseProvider == "Mono.Data.Sqlite")
				return DatabaseType.SQLite;
			else if (Configuration.DatabaseProvider == "Npgsql")
				return DatabaseType.PostgreSQL;
			else
				return DatabaseType.Unknown;
		}

		public bool IsSQLite()
		{
			return Type == DatabaseType.SQLite;
		}
	}
}
