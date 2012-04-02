using System;
using System.Collections.Generic;

namespace RiotGear
{
	public class ExtendedPlayer : Player
	{
		public int InternalGameId;

		public MapType Map;
		public GameModeType GameMode;

		public int GameTime;

		public bool Won;

		public bool BlueTeamWon;

		public bool IsBlueTeam;

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

		public ExtendedPlayer(DatabaseReader reader)
			: base(reader)
		{
		}

		override protected void PerformExtendedReading(DatabaseReader reader)
		{
			InternalGameId = reader.Integer();

			Map = reader.Map();
			GameMode = reader.GameMode();

			GameTime = reader.Integer();

			int blueTeamId = reader.Integer();
			int purpleTeamId = reader.Integer();

			BlueTeamWon = reader.Boolean();

			IsBlueTeam = blueTeamId == TeamId;

			Won = IsBlueTeam == BlueTeamWon;

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
			return Player.GetFields() + ", " + ExtendedFields.FieldString();
		}
	}
}
