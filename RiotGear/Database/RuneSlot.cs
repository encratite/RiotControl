namespace RiotGear
{
	public class RuneSlot
	{
		public int Slot;
		public int Rune;

		static string[] Fields =
		{
			"rune_slot",
			"rune",
		};

		public RuneSlot(DatabaseReader reader)
		{
			Slot = reader.Integer();
			Rune = reader.Integer();

			reader.SanityCheck(Fields);
		}

		public static string GetFields()
		{
			return Fields.FieldString();
		}
	}
}
