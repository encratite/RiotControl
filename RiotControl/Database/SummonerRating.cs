using Npgsql;

namespace RiotControl
{
	class SummonerRating
	{
		public MapType Map;
		public GameModeType GameMode;

		public int Wins;
		public int Losses;
		public int Leaves;

		public int? CurrentRating;
		public int? TopRating;

		static string[] Fields =
		{
			"rating_map",
			"game_mode",

			"wins",
			"losses",
			"leaves",

			"current_rating",
			"top_rating",
		};

		public SummonerRating(NpgsqlDataReader dataReader)
		{
			Reader reader = new Reader(dataReader);

			Map = reader.String().ToMapType();
			GameMode = reader.String().ToGameModeType();

			Wins = reader.Integer();
			Losses = reader.Integer();
			Leaves = reader.Integer();

			CurrentRating = reader.MaybeInteger();
			TopRating = reader.MaybeInteger();

			reader.SanityCheck(Fields);
		}

		public static string GetFields()
		{
			return Fields.FieldString();
		}
	}
}
