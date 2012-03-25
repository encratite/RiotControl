using System.Collections.Generic;
using System.Data.Common;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		List<GameTeamPlayer> LoadSummonerGames(Summoner summoner, DbConnection connection)
		{
			List<GameTeamPlayer> output = new List<GameTeamPlayer>();
			using (var select = Command("select {0} from game, player where game.id = player.game_id and player.summoner_id = :summoner_id order by game.time desc", connection, GameTeamPlayer.GetFields()))
			{
				select.Set("summoner_id", summoner.Id);
				using (var reader = select.ExecuteReader())
				{
					while (reader.Read())
					{
						GameTeamPlayer player = new GameTeamPlayer(reader);
						output.Add(player);
					}
				}
			}
			return output;
		}
	}
}
