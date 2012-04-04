using System;
using System.Data.Common;

using Nil;

namespace RiotGear
{
	public class DatabaseReader : IDisposable
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

		public object Get(string name)
		{
			return DataReader[name];
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
			else if (value.GetType() == typeof(long))
				return (int)(long)value;
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
			return LongInteger().FromUnixTime();
		}

		public MapType Map()
		{
			return (MapType)Integer();
		}

		public GameModeType GameMode()
		{
			return (GameModeType)Integer();
		}

		public bool Read()
		{
			Index = 0;
			return DataReader.Read();
		}

		public void SanityCheck(string[] fields)
		{
			if (fields.Length != Index)
				throw new Exception("Data reader field count mismatch");
		}

		public void Dump(string description)
		{
			Console.WriteLine("{0}:\n", description);
			for (int i = 0; i < DataReader.FieldCount; i++)
			{
				object target = DataReader[i];
				Console.WriteLine("[{0} {1}] {2}: {3}", i, DataReader.GetName(i), target.GetType(), target);
			}
		}
	}
}
