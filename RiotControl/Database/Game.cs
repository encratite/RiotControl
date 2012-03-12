using System;

using Npgsql;

namespace RiotControl
{
	class Game
	{
		public int Id;

		public int GameId;

		public MapType Map;
		public GameModeType GameMode;

		public DateTime GameTime;

		public bool Team1Won;

		public int Team1Id;
		public int Team2Id;

		public string[] Fields =
		{
			"id",

			"game_id",

			"result_map",
			"game_mode",

			"game_time",

			"team1_won",

			"team1_id",
			"team2_id",
		};

		public Game(NpgsqlDataReader dataReader)
		{
			Reader reader = new Reader(dataReader);

			Id = reader.Integer();

			GameId = reader.Integer();

			Map = reader.String().ToMapType();
			GameMode = reader.String().ToGameModeType();

			GameTime = reader.Time();

			Team1Won = reader.Boolean();

			Team1Id = reader.Integer();
			Team2Id = reader.Integer();
		}

		public string GetFields()
		{
			return Fields.FieldString();
		}
	}
}
