﻿using System;
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

		public SQLCommand(string query, NpgsqlConnection connection, params object[] arguments)
		{
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

		public int Execute()
		{
			return Command.ExecuteNonQuery();
		}

		public NpgsqlDataReader ExecuteReader()
		{
			return Command.ExecuteReader();
		}

		public object ExecuteScalar()
		{
			return Command.ExecuteScalar();
		}

		public void CopyParameters(SQLCommand command)
		{
			Command.Parameters.AddRange(command.Command.Parameters.ToArray());
		}
	}
}
