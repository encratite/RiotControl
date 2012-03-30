using System.Collections.Generic;
using System.Data.Common;

using LibOfLegends;

using com.riotgames.platform.statistics;

namespace RiotControl
{
	partial class Worker
	{
		static string[] SummonerRankedStatisticsFields =
		{
			"summoner_id",

			"season",

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

		void SetSummonerRankedStatisticsParameters(DatabaseCommand update, Summoner summoner, int season, ChampionStatistics champion)
		{
			update.SetFieldNames(SummonerRankedStatisticsFields);

			update.Set(summoner.Id);

			update.Set(season);

			update.Set(champion.ChampionId);

			update.Set(champion.Wins);
			update.Set(champion.Losses);

			update.Set(champion.Kills);
			update.Set(champion.Deaths);
			update.Set(champion.Assists);

			update.Set(champion.MinionKills);

			update.Set(champion.Gold);

			update.Set(champion.TurretsDestroyed);

			update.Set(champion.DamageDealt);
			update.Set(champion.PhysicalDamageDealt);
			update.Set(champion.MagicalDamageDealt);

			update.Set(champion.DamageTaken);

			update.Set(champion.DoubleKills);
			update.Set(champion.TripleKills);
			update.Set(champion.QuadraKills);
			update.Set(champion.PentaKills);

			update.Set(champion.TimeSpentDead);

			update.Set(champion.MaximumKills);
			update.Set(champion.MaximumDeaths);
		}

		void UpdateSummonerRankedStatistics(Summoner summoner, int season, AggregatedStats aggregatedStatistics, DbConnection connection)
		{
			List<ChampionStatistics> statistics = ChampionStatistics.GetChampionStatistics(aggregatedStatistics);
			foreach (var champion in statistics)
			{
				using (var championUpdate = Command("update summoner_ranked_statistics set wins = :wins, losses = :losses, kills = :kills, deaths = :deaths, assists = :assists, minion_kills = :minion_kills, gold = :gold, turrets_destroyed = :turrets_destroyed, damage_dealt = :damage_dealt, physical_damage_dealt = :physical_damage_dealt, magical_damage_dealt = :magical_damage_dealt, damage_taken = :damage_taken, double_kills = :double_kills, triple_kills = :triple_kills, quadra_kills = :quadra_kills, penta_kills = :penta_kills, time_spent_dead = :time_spent_dead, maximum_kills = :maximum_kills, maximum_deaths = :maximum_deaths where summoner_id = :summoner_id and season = :season and champion_id = :champion_id", connection))
				{
					SetSummonerRankedStatisticsParameters(championUpdate, summoner, season, champion);

					int rowsAffected = championUpdate.Execute();

					if (rowsAffected == 0)
					{
						//The champion entry didn't exist yet so we must create a new entry first
						string query = string.Format("insert into summoner_ranked_statistics ({0}) values ({1})", GetGroupString(SummonerRankedStatisticsFields), GetPlaceholderString(SummonerRankedStatisticsFields));
						using (var championInsert = Command(query, connection))
						{
							SetSummonerRankedStatisticsParameters(championInsert, summoner, season, champion);
							championInsert.Execute();
						}
					}
				}
			}
		}
	}
}
