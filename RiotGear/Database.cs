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
			Factory = DbProviderFactories.GetFactory(provider);
			Path = configuration.Database;
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
