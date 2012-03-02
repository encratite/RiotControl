using System;
using System.Collections.Generic;

using Npgsql;
using NpgsqlTypes;

namespace RiotControl
{
	static class ParameterExtension
	{
		public static void Add(this NpgsqlParameterCollection parameters, string name, NpgsqlDbType type)
		{
			parameters.Add(new NpgsqlParameter(name, type));
		}

		public static void Set(this NpgsqlParameterCollection parameters, List<string>.Enumerator iterator, NpgsqlDbType type, object value)
		{
			parameters.Add(iterator.Current, type);
			parameters[parameters.Count - 1].Value = value;
			iterator.MoveNext();
		}

		public static void Set(this NpgsqlParameterCollection parameters, List<string>.Enumerator iterator, int value)
		{
			parameters.Set(iterator, NpgsqlDbType.Integer, value);
		}

		public static void Set(this NpgsqlParameterCollection parameters, List<string>.Enumerator iterator, string value)
		{
			parameters.Set(iterator, NpgsqlDbType.Text, value);
		}
	}
}
