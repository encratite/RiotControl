namespace RiotGear
{
	static class Utility
	{
		public static string FieldString(this string[] input)
		{
			return string.Join(", ", input);
		}
	}
}
