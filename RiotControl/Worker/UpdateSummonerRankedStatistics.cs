using System.Collections.Generic;

using LibOfLegends;

using com.riotgames.platform.statistics;

namespace RiotControl
{
	partial class Worker
	{
		void UpdateSummonerRankedStatistics(SummonerDescription summoner, AggregatedStats aggregatedStatistics)
		{
			List<string> fields = new List<string>()
			{
				"summoner_id",

				"champion_id",

				"wins",
				"losses",

				"kills",
				"deaths",
				"assists",

				"minion_kills",
				
				"gold",
				
				"turrets_destroyed",
				
				"damage_dealt",
				"physical_damage_dealt",
				"magical_damage_dealt",
				
				"damage_taken",
				
				"double_kills",
				"triple_kills",
				"quadra_kills",
				"penta_kills",
				
				"time_spent_dead",
				
				"maximum_kills",
				"maximum_deaths",
			};

			List<ChampionStatistics> statistics = ChampionStatistics.GetChampionStatistics(aggregatedStatistics);
			foreach (var champion in statistics)
			{
				DatabaseCommand championUpdate = Command("update summoner_ranked_statistics set wins = :wins, losses = :losses, kills = :kills, deaths = :deaths, assists = :assists, minion_kills = :minion_kills, gold = :gold, turrets_destroyed = :turrets_destroyed, damage_dealt = :damage_dealt, physical_damage_dealt = :physical_damage_dealt, magical_damage_dealt = :magical_damage_dealt, damage_taken = :damage_taken, double_kills = :double_kills, triple_kills = :triple_kills, quadra_kills = :quadra_kills, penta_kills = :penta_kills, time_spent_dead = :time_spent_dead, maximum_kills = :maximum_kills, maximum_deaths = :maximum_deaths where summoner_id = :summoner_id and champion_id = :champion_id");
				championUpdate.SetFieldNames(fields);

				championUpdate.Set(summoner.Id);

				championUpdate.Set(champion.ChampionId);

				championUpdate.Set(champion.Wins);
				championUpdate.Set(champion.Losses);

				championUpdate.Set(champion.Kills);
				championUpdate.Set(champion.Deaths);
				championUpdate.Set(champion.Assists);

				championUpdate.Set(champion.MinionKills);

				championUpdate.Set(champion.Gold);

				championUpdate.Set(champion.TurretsDestroyed);

				championUpdate.Set(champion.DamageDealt);
				championUpdate.Set(champion.PhysicalDamageDealt);
				championUpdate.Set(champion.MagicalDamageDealt);

				championUpdate.Set(champion.DamageTaken);

				championUpdate.Set(champion.DoubleKills);
				championUpdate.Set(champion.TripleKills);
				championUpdate.Set(champion.QuadraKills);
				championUpdate.Set(champion.PentaKills);

				championUpdate.Set(champion.TimeSpentDead);

				championUpdate.Set(champion.MaximumKills);
				championUpdate.Set(champion.MaximumDeaths);

				int rowsAffected = championUpdate.Execute();

				if (rowsAffected == 0)
				{
					//The champion entry didn't exist yet so we must create a new entry first
					string queryFields = GetGroupString(fields);
					string queryValues = GetPlaceholderString(fields);
					DatabaseCommand championInsert = Command(string.Format("insert into summoner_ranked_statistics ({0}) values ({1})", queryFields, queryValues));
					championInsert.CopyParameters(championUpdate);
					championInsert.Execute();
				}
			}
		}
	}
}
