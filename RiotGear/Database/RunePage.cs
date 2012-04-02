using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace RiotGear
{
	public class RunePage
	{
		[ScriptIgnore]
		public int Id;
		public string Name;
		public bool IsCurrentRunePage;
		public int TimeCreated;
		public List<RuneSlot> Slots;

		static string[] Fields =
		{
			"id",
			"name",
			"is_current_rune_page",
			"time_created",
		};

		public RunePage(DatabaseReader reader)
		{
			Id = reader.Integer();
			Name = reader.String();
			IsCurrentRunePage = reader.Boolean();
			TimeCreated = reader.Integer();

			Slots = new List<RuneSlot>();

			reader.SanityCheck(Fields);
		}

		public static string GetFields()
		{
			return Fields.FieldString();
		}
	}
}
