using System;

namespace RiotControl
{
	class Game
	{
		public int Id;

		public int GameId;

		public MapType Map;
		public GameModeType GameMode;

		public DateTime GameTime;

		public int BlueTeamId;
		public int PurpleTeamId;

		public bool BlueTeamWon;

		public static string[] Fields =
		{
			"id",

			"game_id",

			"map",
			"game_mode",

			"game_time",

			"blue_team_id",
			"purple_team_id",

			"blue_team_won",
		};

		public Game(DatabaseReader reader)
		{
			Id = reader.Integer();

			GameId = reader.Integer();

			Map = (MapType)reader.Integer();
			GameMode = (GameModeType)reader.Integer();

			GameTime = reader.Time();

			BlueTeamWon = reader.Boolean();

			BlueTeamId = reader.Integer();
			PurpleTeamId = reader.Integer();

			reader.SanityCheck(Fields);
		}

		public static string GetFields()
		{
			return Fields.FieldString();
		}
	}
}
