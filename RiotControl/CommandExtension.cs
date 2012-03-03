using System;
using System.Collections.Generic;

using Npgsql;
using NpgsqlTypes;

namespace RiotControl
{
	static class CommandExtension
	{
		public static void Add(this NpgsqlCommand command, string name, NpgsqlDbType type)
		{
			command.Parameters.Add(new NpgsqlParameter(name, type));
		}

		public static void Set(this NpgsqlCommand command, string name, NpgsqlDbType type, object value)
		{
			command.Parameters.Add(name, type);
			command.Parameters[command.Parameters.Count - 1].Value = value;
		}

		public static void Set(this NpgsqlCommand command, string name, int value)
		{
			command.Set(name, NpgsqlDbType.Integer, value);
		}

		public static void Set(this NpgsqlCommand command, string name, string value)
		{
			command.Set(name, NpgsqlDbType.Text, value);
		}

		public static void SetEnum(this NpgsqlCommand command, string name, string value)
		{
			command.Set(name, NpgsqlDbType.Varchar, value);
		}

		public static void Set(this NpgsqlCommand command, ref List<string>.Enumerator iterator, NpgsqlDbType type, object value)
		{
			iterator.MoveNext();
			command.Set(iterator.Current, type, value);
		}

		public static void Set(this NpgsqlCommand command, ref List<string>.Enumerator iterator, int value)
		{
			command.Set(ref iterator, NpgsqlDbType.Integer, value);
		}

		public static void Set(this NpgsqlCommand command, ref List<string>.Enumerator iterator, string value)
		{
			command.Set(ref iterator, NpgsqlDbType.Text, value);
		}

		public static void SetEnum(this NpgsqlCommand command, ref List<string>.Enumerator iterator, string value)
		{
			command.Set(ref iterator, NpgsqlDbType.Varchar, value);
		}
	}
}
