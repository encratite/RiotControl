using System;
using System.Collections.Generic;

namespace RiotControl
{
	class GameTeamPlayer : TeamPlayer
	{
		public int InternalGameId;

		public MapType Map;
		public GameModeType GameMode;

		public DateTime GameTime;

		static string[] ExtendedFields =
		{
			"game_result.game_id",

			"map",
			"game_mode",

			"game_time",
		};

		public GameTeamPlayer(DatabaseReader reader)
			: base(reader)
		{
		}

		override protected void PerformExtendedReading(DatabaseReader reader)
		{
			InternalGameId = reader.Integer();

			Map = (MapType)reader.Integer();
			GameMode = (GameModeType)reader.Integer();

			GameTime = reader.Time();

			reader.SanityCheck(GetExtendedFields());
		}

		static string[] GetExtendedFields()
		{
			var fields = new List<string>(Fields);
			fields.AddRange(ExtendedFields);
			return fields.ToArray();
		}

		public new static string GetFields()
		{
			return TeamPlayer.GetFields() + ", " + ExtendedFields.FieldString();
		}
	}
}
