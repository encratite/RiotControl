using System;
using System.Collections.Generic;

namespace RiotControl
{
	public class GameTeamPlayer : TeamPlayer
	{
		public int InternalGameId;

		public MapType Map;
		public GameModeType GameMode;

		public DateTime GameTime;

		public int BlueTeamId;
		public int PurpleTeamId;

		public bool BlueTeamWon;

		static string[] ExtendedFields =
		{
			"game.game_id",

			"map",
			"game_mode",

			"time",

			"blue_team_id",
			"purple_team_id",

			"blue_team_won",
		};

		public GameTeamPlayer(DatabaseReader reader)
			: base(reader)
		{
		}

		override protected void PerformExtendedReading(DatabaseReader reader)
		{
			InternalGameId = reader.Integer();

			Map = reader.Map();
			GameMode = reader.GameMode();

			GameTime = reader.Time();

			BlueTeamId = reader.Integer();
			PurpleTeamId = reader.Integer();

			BlueTeamWon = reader.Boolean();

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
