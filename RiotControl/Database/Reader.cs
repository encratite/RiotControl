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
			return (int)Get();
		}

		public int? MaybeInteger()
		{
			return (int?)Get();
		}

		public string String()
		{
			return (string)Get();
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
