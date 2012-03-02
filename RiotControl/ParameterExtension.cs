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

		public static void Set(this NpgsqlParameterCollection parameters, string name, NpgsqlDbType type, object value)
		{
			parameters.Add(name, type);
			parameters[parameters.Count - 1].Value = value;
		}

		public static void Set(this NpgsqlParameterCollection parameters, ref List<string>.Enumerator iterator, NpgsqlDbType type, object value)
		{
			iterator.MoveNext();
			parameters.Set(iterator.Current, type, value);
		}

		public static void Set(this NpgsqlParameterCollection parameters, ref List<string>.Enumerator iterator, int value)
		{
			parameters.Set(ref iterator, NpgsqlDbType.Integer, value);
		}

		public static void Set(this NpgsqlParameterCollection parameters, ref List<string>.Enumerator iterator, string value)
		{
			parameters.Set(ref iterator, NpgsqlDbType.Text, value);
		}
	}
}
