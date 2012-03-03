using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Npgsql;
using NpgsqlTypes;

using LibOfLegends;

using com.riotgames.platform.statistics;
using com.riotgames.platform.summoner;

namespace RiotControl
{
	class RegionHandler
	{
		EngineRegionProfile RegionProfile;
		NpgsqlConnection Database;
		RPCService RPC;

		public RegionHandler(Configuration configuration, EngineRegionProfile regionProfile, NpgsqlConnection database)
		{
			RegionProfile = regionProfile;
			Database = database;
			if (regionProfile.Logins.Count != 1)
				throw new Exception("Currently the number of accounts per region is limited to one");
			Login login = regionProfile.Logins.First();
			ConnectionProfile connectionData = new ConnectionProfile(configuration.Authentication, regionProfile.Region, configuration.Proxy, login.Username, login.Password);
			RPC = new RPCService(connectionData);
			WriteLine("Connecting to the server");
			RPC.Connect(OnConnect);
		}

		void WriteLine(string input, params object[] arguments)
		{
			Nil.Output.WriteLine(string.Format("{0} [{1}] {2}", Nil.Time.Timestamp(), RegionProfile.Abbreviation, input), arguments);
		}

		void SummonerMessage(string message, Summoner summoner, params object[] arguments)
		{
			WriteLine(string.Format("{0} ({1}): {2}", summoner.Name, summoner.AccountId, message), arguments);
		}

		void OnConnect(bool connected)
		{
			if (connected)
			{
				WriteLine("Successfully connected to the server");
				//OnConnect must return so we can't use the current thread to execute region handler logic.
				//This is a limitation imposed by FluorineFX.
				(new Thread(Run)).Start();
			}
			else
				WriteLine("There was an error connecting to the server");
		}

		void ProcessSummary(string mapEnum, string gameModeEnum, string target, Summoner summoner, List<PlayerStatSummary> summaries, bool forceNullRating = false)
		{
			foreach (var summary in summaries)
			{
				if (summary.playerStatSummaryType != target)
					continue;
				NpgsqlCommand update = new NpgsqlCommand("update summoner_rating set wins = :wins, losses = :losses, leaves = :leaves, current_rating = :current_rating, top_rating = :top_rating where summoner_id = :summoner_id and rating_map = cast(:rating_map as map_type) and game_mode = cast(:game_mode as game_mode_type)", Database);
				if (forceNullRating)
				{
					update.Set("current_rating", NpgsqlDbType.Integer, null);
					update.Set("top_rating", NpgsqlDbType.Integer, null);
				}
				else
				{
					update.Set("current_rating", summary.rating);
					update.Set("top_rating", summary.maxRating);
				}

				update.Set("summoner_id", summoner.Id);
				update.SetEnum("rating_map", mapEnum);
				update.SetEnum("game_mode", gameModeEnum);

				update.Set("wins", summary.wins);
				update.Set("losses", summary.losses);
				update.Set("leaves", summary.leaves);

				int rowsAffected = update.ExecuteNonQuery();
				if (rowsAffected == 0)
				{
					//We're dealing with a new summoner rating entry, insert it
					NpgsqlCommand insert = new NpgsqlCommand("insert into summoner_rating (summoner_id, rating_map, game_mode, wins, losses, leaves, current_rating, top_rating) values (:summoner_id, cast(:rating_map as map_type), cast(:game_mode as game_mode_type), :wins, :losses, :leaves, :current_rating, :top_rating)", Database);
					insert.Parameters.AddRange(update.Parameters.ToArray());
					insert.ExecuteNonQuery();
					SummonerMessage(string.Format("New rating for mode {0}", target), summoner);
				}
				else
				{
					//This rating was already in the database and was updated
					SummonerMessage(string.Format("Updated rating for mode {0}", target), summoner);
				}
				break;
			}
		}

		void UpdateSummonerLastModifiedTimestamp(Summoner summoner)
		{
			NpgsqlCommand timeUpdate = new NpgsqlCommand(string.Format("update summoner set time_updated = {0} where id = :id", CurrentTimestamp()), Database);
			timeUpdate.Set("id", summoner.Id);
			timeUpdate.ExecuteNonQuery();
		}

		void UpdateSummonerRatings(Summoner summoner, PlayerLifeTimeStats lifeTimeStatistics)
		{
			List<PlayerStatSummary> summaries = lifeTimeStatistics.playerStatSummaries.playerStatSummarySet;

			ProcessSummary("summoners_rift", "normal", "Unranked", summoner, summaries, true);
			ProcessSummary("twisted_treeline", "premade", "RankedPremade3x3", summoner, summaries);
			ProcessSummary("summoners_rift", "solo", "RankedSolo5x5", summoner, summaries);
			ProcessSummary("summoners_rift", "premade", "RankedPremade5x5", summoner, summaries);
			ProcessSummary("dominion", "normal", "OdinUnranked", summoner, summaries);
		}

		void UpdateSummonerRankedStatistics(Summoner summoner, AggregatedStats aggregatedStatistics)
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
				SQLCommand championUpdate = new SQLCommand("update summoner_ranked_statistics set wins = :wins, losses = :losses, kills = :kills, deaths = :deaths, assists = :assists, minion_kills = :minion_kills, gold = :gold, turrets_destroyed = :turrets_destroyed, damage_dealt = :damage_dealt, physical_damage_dealt = :physical_damage_dealt, magical_damage_dealt = :magical_damage_dealt, damage_taken = :damage_taken, double_kills = :double_kills, triple_kills = :triple_kills, quadra_kills = :quadra_kills, penta_kills = :penta_kills, time_spent_dead = :time_spent_dead, maximum_kills = :maximum_kills, maximum_deaths = :maximum_deaths where summoner_id = :summoner_id and champion_id = :champion_id", Database);
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
					SQLCommand championInsert = new SQLCommand(string.Format("insert into summoner_ranked_statistics ({0}) values ({1})", queryFields, queryValues), Database);
					championInsert.CopyParameters(championUpdate);
					championInsert.Execute();
				}
			}
		}

		double GetTimestamp(DateTime input)
		{
			DateTime epoch = new DateTime(1970, 1, 1).ToLocalTime();
			TimeSpan difference = input - epoch;
			return difference.TotalSeconds;
		}

		void InsertGameResult(Summoner summoner, PlayerGameStats game, GameResult gameResult)
		{
			List<string> fields = new List<string>()
			{
				"team_id",
				"summoner_id",

				"ping",
				"time_spent_in_queue",

				"premade_size",

				"k_coefficient",
				"probability_of_winning",

				"rating",
				"rating_change",
				"adjusted_rating",

				"experience_earned",
				"boosted_experience_earned",

				"ip_earned",
				"boosted_ip_earned",

				"summoner_level",

				"summoner_spell1",
				"summoner_spell2",

				"champion_id",

				"skin_name",
				"skin_index",

				"champion_level",

				//"items",

				"kills",
				"deaths",
				"assists",

				"minion_kills",
				"neutral_minions_killed",

				"gold",

				"turrets_destroyed",
				"inhibitors_destroyed",

				"damage_dealt",
				"physical_damage_dealt",
				"magical_damage_dealt",

				"damage_taken",
				"physical_damage_taken",
				"magical_damage_taken",

				"total_healing_done",

				"time_spent_dead",

				"largest_multikill",
				"largest_killing_spree",
				"largest_critical_strike",
			};

			string queryFields = GetGroupString(fields);
			string queryValues = GetPlaceholderString(fields);
			string arrayString = "{:item0, :item1, :item2, :item3, :item4, :item5}";

			SQLCommand insert = new SQLCommand("insert into team_player ({0}, items) values ({1}, '{2}')", Database, queryFields, queryValues, arrayString);

			insert.Set(game.gameId);
			insert.Set(summoner.Id);

			insert.Set(game.userServerPing);
			insert.Set(game.timeInQueue);

			insert.Set(game.premadeSize);

			insert.Set(game.KCoefficient);
			insert.Set(game.predictedWinPct);

			insert.Set(game.rating);
			insert.Set(game.eloChange);
			insert.Set(game.adjustedRating);

			insert.Set(game.experienceEarned);
			insert.Set(game.boostXpEarned);

			insert.Set(game.ipEarned);
			insert.Set(game.boostIpEarned);

			insert.Set(game.level);

			insert.Set(game.spell1);
			insert.Set(game.spell2);

			insert.Set(game.championId);

			insert.Set(game.skinName);
			insert.Set(game.skinIndex);

			insert.Set(gameResult.Level);

			//Items require special treatment

			insert.Set(gameResult.Kills);
			insert.Set(gameResult.Deaths);
			insert.Set(gameResult.Assists);

			insert.Set(gameResult.MinionsKilled);
			insert.Set(gameResult.NeutralMinionsKilled);

			insert.Set(gameResult.GoldEarned);

			insert.Set(gameResult.TurretsDestroyed);
			insert.Set(gameResult.InhibitorsDestroyed);

			insert.Set(gameResult.TotalDamageDealt);
			insert.Set(gameResult.PhysicalDamageDealt);
			insert.Set(gameResult.MagicalDamageDealt);

			insert.Set(gameResult.TotalDamageTaken);
			insert.Set(gameResult.PhysicalDamageTaken);
			insert.Set(gameResult.MagicalDamageTaken);

			insert.Set(gameResult.TotalHealingDone);

			insert.Set(gameResult.TimeSpentDead);

			insert.Set(gameResult.LargestMultiKill);
			insert.Set(gameResult.LargestKillingSpree);
			insert.Set(gameResult.LargestCriticalStrike);

			for(int i = 0; i < gameResult.Items.Length; i++)
				insert.Set(string.Format("item{0}", i), gameResult.Items[i]);

			insert.Execute();
		}

		void UpdateSummonerGames(Summoner summoner, PlayerGameStats game)
		{
			GameResult gameResult = new GameResult(game);
			//At first we must determine if the game is already in the database
			SQLCommand check = new SQLCommand("select game_result.id, game_result.team1_id, game_result.team2_id from game_result, team as team1, team as team2 where game_result.team1_id = team1.id and game_result.team2_id = team2.id and (team1.id = :team_id or team2.id = :team_id)", Database);
			check.Set("team_id", game.teamId);
			var reader = check.ExecuteReader();
			if (reader.Read())
			{
				//The game is already in the database
				int gameId = (int)reader[0];
				int team1Id = (int)reader[1];
				int team2Id = (int)reader[2];
				//Store the team ID, just in case it had not been set yet because this player might have been on the enemy team originally
				SQLCommand setTeamId = new SQLCommand("update team set team_id = :team_id where id = :id", Database);
				setTeamId.Set("team_id", game.teamId);
				setTeamId.Set("id", gameId);
				//Check if the game result for this player has already been stored
				SQLCommand gameCheck = new SQLCommand("select count(*) from team_player where (team_id = :team1_id or team_id = :team2_id) and summoner_id = :summoner_id", Database);
				gameCheck.Set("team1_id", team1Id);
				gameCheck.Set("team2_id", team2Id);
				gameCheck.Set("summoner_id", summoner.Id);
				int count = (int)gameCheck.ExecuteScalar();
				if (count > 0)
				{
					//The result of this game for this player has already been stored in the database, there is no work to be done
					return;
				}
				//The game is already stored in the database but the results of this player were previously unknown
			}
			else
			{
				//The game is not in the database yet
				//Need to create the team entries first
				SQLCommand newTeam1 = new SQLCommand("insert into team (team_id) values (:team_id)", Database);
				newTeam1.Set("team_id", game.teamId);
				newTeam1.Execute();
				int team1Id = GetInsertId("team");
				SQLCommand newTeam2 = new SQLCommand("insert into team (team_id) values (null)", Database);
				newTeam2.Execute();
				int team2Id = GetInsertId("team");
				List<string> fields = new List<string>()
					{
						"game_id",
						"result_map",
						"game_mode",
						"game_time",
						"team1_won",
						"team1_id",
						"team2_id",
					};
				string mapEnum;
				string gameModeEnum;
				switch (game.gameMapId)
				{
					case 1:
						mapEnum = "summoners_rift";
						break;

					case 4:
						mapEnum = "twisted_treeline";
						break;

					case 8:
						mapEnum = "dominion";
						break;

					default:
						throw new Exception(string.Format("Unknown game map ID in the match history of {0}: {1}", summoner.Name, game.gameMapId));
				}
				if (game.gameType == "PRACTICE_GAME")
					gameModeEnum = "custom";
				else
				{
					switch (game.queueType)
					{
						case "RANKED_TEAM_3x3":
						case "RANKED_TEAM_5x5":
							gameModeEnum = "premade";
							break;

						case "NORMAL":
						case "ODIN_UNRANKED":
							gameModeEnum = "normal";
							break;

						case "RANKED_SOLO_5x5":
							gameModeEnum = "solo";
							break;

						case "BOT":
							gameModeEnum = "bot";
							break;

						default:
							throw new Exception(string.Format("Unknown queue type in the match history of {0}: {1}", summoner.Name, game.queueType));
					}
				}
				string queryFields = GetGroupString(fields);
				string queryValues = ":game_id, cast(:result_map as map_type), cast(:game_mode as game_mode_type), to_timestamp(:game_time), :team1_won, :team1_id, :team2_id";
				SQLCommand newGame = new SQLCommand("insert into game_result ({0}) values ({1})", Database, queryFields, queryValues);
				newGame.SetFieldNames(fields);
				newGame.Set(game.gameId);
				newGame.Set(mapEnum);
				newGame.Set(gameModeEnum);
				newGame.Set(GetTimestamp(game.createDate));
				newGame.Set(gameResult.Win);
				newGame.Set(team1Id);
				newGame.Set(team2Id);
			}
			reader.Close();
			InsertGameResult(summoner, game, gameResult);
		}


		void UpdateSummonerGames(Summoner summoner, RecentGames recentGameData)
		{
			foreach (var game in recentGameData.gameStatistics)
				UpdateSummonerGames(summoner, game);
		}

		void UpdateSummoner(Summoner summoner, bool isNewSummoner)
		{
			PlayerLifeTimeStats lifeTimeStatistics = RPC.RetrievePlayerStatsByAccountID(summoner.AccountId, "CURRENT");
			if (lifeTimeStatistics == null)
			{
				SummonerMessage("Unable to retrieve lifetime statistics", summoner);
				return;
			}

			AggregatedStats aggregatedStatistics = RPC.GetAggregatedStats(summoner.AccountId, "CLASSIC", "CURRENT");
			if (aggregatedStatistics == null)
			{
				SummonerMessage("Unable to retrieve aggregated statistics", summoner);
				return;
			}

			RecentGames recentGameData = RPC.GetRecentGames(summoner.AccountId);
			if (recentGameData == null)
			{
				SummonerMessage("Unable to retrieve recent games", summoner);
				return;
			}

			UpdateSummonerRatings(summoner, lifeTimeStatistics);
			UpdateSummonerRankedStatistics(summoner, aggregatedStatistics);
			UpdateSummonerGames(summoner, recentGameData);

			if (!isNewSummoner)
			{
				//This means that the main summoner entry must be updated
				UpdateSummonerLastModifiedTimestamp(summoner);
			}
		}

		string GetGroupString(List<string> fields)
		{
			return String.Join(", ", fields);
		}

		string GetPlaceholderString(List<string> fields)
		{
			var mapped = from x in fields
						 select ":" + x;
			return GetGroupString(mapped.ToList());
		}

		string Zulufy(string input)
		{
			return string.Format("{0} at time zone 'UTC'", input);
		}

		string CurrentTimestamp()
		{
			return Zulufy("current_timestamp");
		}

		void UpdateSummonerByName(string summonerName)
		{
			NpgsqlCommand nameLookup = new NpgsqlCommand("select id, account_id from summoner where region = cast(:region as region_type) and summoner_name = :name", Database);
			nameLookup.SetEnum("region", RegionProfile.RegionEnum);
			nameLookup.Set("name", NpgsqlDbType.Text, summonerName);
			NpgsqlDataReader reader = nameLookup.ExecuteReader();
			if (reader.Read())
			{
				//The summoner already exists in the database
				int id = (int)reader[0];
				int accountId = (int)reader[1];
				UpdateSummoner(new Summoner(summonerName, id, accountId), false);
			}
			else
			{
				//We are dealing with a new summoner
				PublicSummoner summoner = RPC.GetSummonerByName(summonerName);
				if (summoner == null)
				{
					WriteLine("No such summoner: {0}", summonerName);
					return;
				}
				List<string> coreFields = new List<string>()
				{
					"account_id",
					"summoner_id",
					"summoner_name",
					"internal_name",
					"summoner_level",
					"profile_icon",
				};

				List<string> extendedFields = new List<string>()
				{
					"time_created",
					"time_updated",
				};

				var field = coreFields.GetEnumerator();

				string fieldsString = string.Format("region, {0}", GetGroupString(coreFields.Concat(extendedFields).ToList()));
				string placeholderString = GetPlaceholderString(coreFields);
				string valuesString = string.Format("cast(:region as region_type), {0}, {1}, {1}", placeholderString, CurrentTimestamp());
				string query = string.Format("insert into summoner ({0}) values ({1})", fieldsString, valuesString);

				NpgsqlCommand newSummoner = new NpgsqlCommand(query, Database);

				newSummoner.SetEnum("region", RegionProfile.RegionEnum);
				newSummoner.Set(ref field, summoner.acctId);
				newSummoner.Set(ref field, summoner.summonerId);
				newSummoner.Set(ref field, summonerName);
				newSummoner.Set(ref field, summoner.internalName);
				newSummoner.Set(ref field, summoner.summonerLevel);
				newSummoner.Set(ref field, summoner.profileIconId);

				newSummoner.ExecuteNonQuery();

				int id = GetInsertId("summoner");
				UpdateSummoner(new Summoner(summonerName, id, summoner.acctId), true);
			}
			reader.Close();
		}

		int GetInsertId(string tableName)
		{
			NpgsqlCommand currentValue = new NpgsqlCommand(string.Format("select currval('{0}_id_seq')", tableName), Database);
			object result = currentValue.ExecuteScalar();
			long id = (long)result;
			return (int)id;
		}

		void Run()
		{
			NpgsqlCommand getJob = new NpgsqlCommand("select id, summoner_name from lookup_job where region = cast(:region as region_type) order by priority desc, time_added limit 1", Database);
			getJob.SetEnum("region", RegionProfile.RegionEnum);

			NpgsqlCommand deleteJob = new NpgsqlCommand("delete from lookup_job where id = :id", Database);
			deleteJob.Add("id", NpgsqlDbType.Integer);

			while (true)
			{
				NpgsqlTransaction lookupTransaction = Database.BeginTransaction();
				NpgsqlDataReader reader = getJob.ExecuteReader();
				bool success = reader.Read();

				int id;
				String summonerName = null;

				if (success)
				{
					id = (int)reader[0];
					summonerName = (string)reader[1];

					//Delete entry
					//deleteCommand.Parameters[0].Value = id;
					//deleteCommand.ExecuteNonQuery();
				}
				lookupTransaction.Commit();

				if (success)
					UpdateSummonerByName(summonerName);
				else
				{
					WriteLine("No jobs available");
					//Should wait for an event here really
					break;
				}

				reader.Close();
			}
		}
	}
}
