using System.Collections.Generic;
using System.Linq;

using Npgsql;
using NpgsqlTypes;

namespace RiotControl
{
	class SQLCommand
	{
		public string Query;
		public NpgsqlCommand Command;

		List<string>.Enumerator Enumerator;
		Profiler CommandProfiler;

		public SQLCommand(string query, NpgsqlConnection connection, Profiler profiler = null, params object[] arguments)
		{
			CommandProfiler = profiler;
			Query = string.Format(query, arguments);
			Command = new NpgsqlCommand(Query, connection);
		}

		public void SetFieldNames(List<string> fields)
		{
			Enumerator = fields.GetEnumerator();
		}

		public void Add(string name, NpgsqlDbType type)
		{
			Command.Parameters.Add(new NpgsqlParameter(name, type));
		}

		public void Set(string name, NpgsqlDbType type, object value)
		{
			Command.Parameters.Add(name, type);
			Command.Parameters[Command.Parameters.Count - 1].Value = value;
		}

		public void Set(string name, int value)
		{
			Set(name, NpgsqlDbType.Integer, value);
		}

		public void Set(string name, string value)
		{
			Set(name, NpgsqlDbType.Text, value);
		}

		public void SetEnum(string name, string value)
		{
			Set(name, NpgsqlDbType.Varchar, value);
		}

		public void Set(NpgsqlDbType type, object value)
		{
			Enumerator.MoveNext();
			Command.Parameters.Add(Enumerator.Current, type);
			Command.Parameters[Command.Parameters.Count - 1].Value = value;
		}

		public void Set(int value)
		{
			Set(NpgsqlDbType.Integer, value);
		}

		public void Set(int? value)
		{
			object argument;
			if (value.HasValue)
				argument = value.Value;
			else
				argument = null;
			Set(NpgsqlDbType.Integer, argument);
		}

		public void Set(string value)
		{
			Set(NpgsqlDbType.Text, value);
		}

		public void Set(double value)
		{
			Set(NpgsqlDbType.Double, value);
		}

		public void Set(bool value)
		{
			Set(NpgsqlDbType.Boolean, value);
		}

		public void SetEnum(string value)
		{
			Set(NpgsqlDbType.Varchar, value);
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

		public NpgsqlDataReader ExecuteReader()
		{
			Start();
			NpgsqlDataReader reader = Command.ExecuteReader();
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
			Command.Parameters.AddRange(command.Command.Parameters.ToArray());
		}
	}
}
