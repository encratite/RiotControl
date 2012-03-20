using System.Collections.Generic;
using System.Data.Common;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		List<GameTeamPlayer> LoadSummonerGames(Summoner summoner, DbConnection database)
		{
			List<GameTeamPlayer> output = new List<GameTeamPlayer>();
			DatabaseCommand select = GetCommand("select {0} from game_result, player where game_result.id = player.game_id and player.summoner_id = :summoner_id order by game_result.game_time desc", database, GameTeamPlayer.GetFields());
			select.Set("summoner_id", summoner.Id);
			using (var reader = select.ExecuteReader())
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
