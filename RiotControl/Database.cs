using System.Data.Common;
using System.Data.SQLite;

namespace RiotControl
{
	class Database
	{
		DbProviderFactory Factory;
		string Path;

		public Database(string path)
		{
			Factory = DbProviderFactories.GetFactory("System.Data.SQLite");
			Path = path;
		}

		public DbConnection GetConnection()
		{
			DbConnection connection = Factory.CreateConnection();
			connection.ConnectionString = string.Format("Data Source = {0}", Path);
			connection.Open();
			return connection;
		}
	}
}
