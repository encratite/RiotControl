using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Npgsql;

namespace RiotControl
{
	class DatabaseConnectionProvider
	{
		DatabaseConfiguration ProviderConfiguration;

		public DatabaseConnectionProvider(DatabaseConfiguration configuration)
		{
			ProviderConfiguration = configuration;
		}

		public NpgsqlConnection GetConnect()
		{
			NpgsqlConnection connection = new NpgsqlConnection("Server = " + ProviderConfiguration.Host + "; Port = " + ProviderConfiguration.Port + "; User Id = " + ProviderConfiguration.Username + "; Database = " + ProviderConfiguration.Database + "; Preload Reader = true; Pooling = true; Minpoolsize = " + ProviderConfiguration.MinimumPoolSize + "; Maxpoolsize = " + ProviderConfiguration.MaximumPoolSize + ";");
			connection.Open();
			return connection;
		}
	}
}
