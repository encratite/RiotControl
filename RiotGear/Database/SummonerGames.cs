using System.Collections.Generic;
using System.Data.Common;

using Blighttp;

namespace RiotGear
{
	partial class WebService
	{
		List<ExtendedPlayer> LoadSummonerGames(Summoner summoner, DbConnection connection)
		{
			List<ExtendedPlayer> output = new List<ExtendedPlayer>();
			using (var select = Command("select {0} from game, player where game.id = player.game_id and player.summoner_id = :summoner_id order by game.time desc", connection, ExtendedPlayer.GetFields()))
			{
				select.Set("summoner_id", summoner.Id);
				using (var reader = select.ExecuteReader())
				{
					while (reader.Read())
					{
						ExtendedPlayer player = new ExtendedPlayer(reader);
						output.Add(player);
					}
				}
			}
			return output;
		}
	}
}
