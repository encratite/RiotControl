using System.Collections.Generic;
using System.Linq;

using System.Data;
using System.Data.Common;

namespace RiotControl
{
	class SQLCommand
	{
		public string Query;
		public DbCommand Command;

		List<string>.Enumerator Enumerator;
		Profiler CommandProfiler;

		public SQLCommand(string query, DbConnection connection, Profiler profiler = null, params object[] arguments)
		{
			CommandProfiler = profiler;
			Query = string.Format(query, arguments);
			Command = connection.CreateCommand();
			Command.CommandText = query;
		}

		public void SetFieldNames(List<string> fields)
		{
			Enumerator = fields.GetEnumerator();
		}

		public void Add(string name, DbType type)
		{
			DbParameter parameter = Command.CreateParameter();
			parameter.ParameterName = name;
			parameter.DbType = type;
			Command.Parameters.Add(parameter);
		}

		public void Set(string name, DbType type, object value)
		{
			Add(name, type);
			Command.Parameters[Command.Parameters.Count - 1].Value = value;
		}

		public void Set(string name, int value)
		{
			Set(name, DbType.Int32, value);
		}

		public void Set(string name, string value)
		{
			Set(name, DbType.String, value);
		}

		public void Set(DbType type, object value)
		{
			Enumerator.MoveNext();
			Set(Enumerator.Current, type, value);
		}

		public void Set(int value)
		{
			Set(DbType.Int32, value);
		}

		public void Set(int? value)
		{
			object argument;
			if (value.HasValue)
				argument = value.Value;
			else
				argument = null;
			Set(DbType.Int32, argument);
		}

		public void Set(string value)
		{
			Set(DbType.String, value);
		}

		public void Set(double value)
		{
			Set(DbType.Double, value);
		}

		public void Set(bool value)
		{
			Set(DbType.Boolean, value);
		}

		void Start()
		{
			if (CommandProfiler != null)
				CommandProfiler.Start(Query);
		}

		void Stop()
		{
			if (CommandProfiler != null)
				CommandProfiler.Stop();
		}

		public int Execute()
		{
			Start();
			int rowsAffected = Command.ExecuteNonQuery();
			Stop();
			return rowsAffected;
		}

		public DbDataReader ExecuteReader()
		{
			Start();
			DbDataReader reader = Command.ExecuteReader();
			Stop();
			return reader;
		}

		public object ExecuteScalar()
		{
			Start();
			object output = Command.ExecuteScalar();
			Stop();
			return output;
		}

		public void CopyParameters(SQLCommand command)
		{
			DbParameter[] parameters = new DbParameter[command.Command.Parameters.Count];
			command.Command.Parameters.CopyTo(parameters, 0);
			Command.Parameters.AddRange(parameters);
		}
	}
}
