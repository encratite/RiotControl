using System.Collections.Generic;

using Blighttp;

using Npgsql;

namespace RiotControl
{
	partial class WebService
	{
		List<GameTeamPlayer> LoadSummonerGames(Summoner summoner, NpgsqlConnection database)
		{
			List<GameTeamPlayer> output = new List<GameTeamPlayer>();
			SQLCommand select = GetCommand("select {0} from game_result, team_player where game_result.id = team_player.game_id and team_player.summoner_id = :summoner_id order by game_result.game_time desc", database, GameTeamPlayer.GetFields());
			select.Set("summoner_id", summoner.Id);
			using (NpgsqlDataReader reader = select.ExecuteReader())
			{
				while (reader.Read())
				{
					GameTeamPlayer player = new GameTeamPlayer(reader);
					output.Add(player);
				}
			}
			return output;
		}
	}
}
