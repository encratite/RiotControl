using System;
using System.Data.Common;

namespace RiotControl
{
	class DatabaseReader : IDisposable
	{
		int Index;
		DbDataReader DataReader;

		public DatabaseReader(DbDataReader reader)
		{
			Index = 0;
			DataReader = reader;
		}

		//For IDisposable
		public void Dispose()
		{
			DataReader.Dispose();
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
			if (value.GetType() == typeof(long))
				return (int)(long)value;
			else
				return (int)value;
		}

		public long LongInteger()
		{
			object value = Get();
			if (value.GetType() == typeof(int))
				return (long)(int)value;
			else
				return (long)value;
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
			return Integer() == 1;
		}

		public double Double()
		{
			return (double)Get();
		}

		public DateTime Time()
		{
			long timestamp = LongInteger();
			DateTime output = new DateTime(1970, 1, 1);
			output.AddSeconds(timestamp);
			return output;
		}

		public bool Read()
		{
			return DataReader.Read();
		}

		public void SanityCheck(string[] fields)
		{
			if (fields.Length != Index)
				throw new Exception("Data reader field count mismatch");
		}
	}
}
