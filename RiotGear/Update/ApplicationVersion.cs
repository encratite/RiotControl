using System;

namespace RiotGear
{
	public class ApplicationVersion : IComparable
	{
		public readonly string Filename;
		public readonly int Revision;

		public ApplicationVersion(string filename, int revision)
		{
			Filename = filename;
			Revision = revision;
		}

		public int CompareTo(object other)
		{
			ApplicationVersion otherVersion = (ApplicationVersion)other;
			return - Revision.CompareTo(otherVersion.Revision);
		}
	}
}
