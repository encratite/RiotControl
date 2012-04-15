using System.Data;
using System.Data.Common;

namespace RiotGear
{
	static class DbConnectionExtension
	{
		public static bool IsMySQL(this DbConnection connection)
		{
			return connection.GetType().Name == "MySqlConnection";
		}
	}
}
