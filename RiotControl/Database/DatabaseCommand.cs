using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace RiotControl
{
	class DatabaseCommand : IDisposable
	{
		public string Query;
		public DbCommand Command;

		string[] Fields;
		int FieldIndex;
		Profiler CommandProfiler;

		public DatabaseCommand(string query, DbConnection connection, Profiler profiler = null, params object[] arguments)
		{
			CommandProfiler = profiler;
			Query = string.Format(query, arguments);
			Command = connection.CreateCommand();
			Command.CommandText = Query;
		}

		//For IDisposable
		public void Dispose()
		{
			Command.Dispose();
		}

		public void SetFieldNames(string[] fields)
		{
			FieldIndex = 0;
			Fields = fields;
		}

		#region Non-enumerated variants

		public void Add(string name, DbType type)
		{
			DbParameter parameter = Command.CreateParameter();
			parameter.ParameterName = string.Format(":{0}", name);
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

		public void Set(string name, long value)
		{
			Set(name, DbType.Int64, value);
		}

		public void Set(string name, string value)
		{
			Set(name, DbType.String, value);
		}

		public void Set(string name, MapType map)
		{
			Set(name, (int)map);
		}

		public void Set(string name, GameModeType gameMode)
		{
			Set(name, (int)gameMode);
		}

		#endregion

		#region Enumerated variants

		public void Set(DbType type, object value)
		{
			string field = Fields[FieldIndex];
			FieldIndex++;
			Set(field, type, value);
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

		public void Set(MapType map)
		{
			Set((int)map);
		}

		public void Set(GameModeType gameMode)
		{
			Set((int)gameMode);
		}

		#endregion

		#region Profiling

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

		#endregion

		#region Execution

		public int Execute()
		{
			Start();
			int rowsAffected = Command.ExecuteNonQuery();
			Stop();
			return rowsAffected;
		}

		public DatabaseReader ExecuteReader()
		{
			Start();
			DbDataReader reader = Command.ExecuteReader();
			Stop();
			DatabaseReader wrappedReader = new DatabaseReader(reader);
			return wrappedReader;
		}

		public object ExecuteScalar()
		{
			Start();
			object output = Command.ExecuteScalar();
			Stop();
			return output;
		}

		#endregion Enumerated variants
	}
}
