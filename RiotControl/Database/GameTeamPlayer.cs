using System;
using System.Collections.Generic;

using Npgsql;

namespace RiotControl
{
	class GameTeamPlayer : TeamPlayer
	{
		public MapType Map;
		public GameModeType GameMode;

		public DateTime GameTime;

		static string[] ExtendedFields =
		{
			"result_map",
			"game_mode",

			"game_time",
		};

		public GameTeamPlayer(NpgsqlDataReader dataReader)
			: base(dataReader)
		{
		}

		override protected void PerformExtendedReading(Reader reader)
		{
			Map = reader.String().ToMapType();
			GameMode = reader.String().ToGameModeType();

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
