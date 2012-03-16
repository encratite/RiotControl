using System;

using Npgsql;

namespace RiotControl
{
	class Reader
	{
		int Index;
		NpgsqlDataReader DataReader;

		public Reader(NpgsqlDataReader reader)
		{
			Index = 0;
			DataReader = reader;
		}

		public object Get()
		{
			object output = DataReader[Index];
			Index++;
			return output;
		}

		public int Integer()
		{
			object value = Get();
			//Hack for aggregates
			if (value.GetType() == typeof(long))
				return (int)(long)value;
			else
				return (int)value;
		}

		public int? MaybeInteger()
		{
			object value = Get();
			if (value.GetType() == typeof(DBNull))
				return null;
			else
				return (int)value;
		}

		public string String()
		{
			object value = Get();
			if (value.GetType() == typeof(DBNull))
				return null;
			else
				return (string)value;
		}

		public bool Boolean()
		{
			return (bool)Get();
		}

		public double Double()
		{
			return (double)Get();
		}

		public DateTime Time()
		{
			return (DateTime)Get();
		}

		public void SanityCheck(string[] fields)
		{
			if (fields.Length != Index)
				throw new Exception("Data reader field count mismatch");
		}
	}
}
