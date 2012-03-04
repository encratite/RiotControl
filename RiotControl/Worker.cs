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
	class Worker
	{
		EngineRegionProfile RegionProfile;
		NpgsqlConnection Database;
		RPCService RPC;
		//This set holds the account IDs that are currently being worked on
		//This way we can avoid updating an account from multiple workers simultaneously, causing concurrency issues with database updates
		HashSet<int> ActiveAccountIds;

		public Worker(Configuration configuration, EngineRegionProfile regionProfile, DatabaseConfiguration databaseConfiguration)
		{
			RegionProfile = regionProfile;
			InitialiseDatabase(databaseConfiguration);
			ActiveAccountIds = new HashSet<int>();
			if (regionProfile.Logins.Count != 1)
				throw new Exception("Currently the number of accounts per region is limited to one");
			Login login = regionProfile.Logins.First();
			ConnectionProfile connectionData = new ConnectionProfile(configuration.Authentication, regionProfile.Region, configuration.Proxy, login.Username, login.Password);
			RPC = new RPCService(connectionData);
			WriteLine("Connecting to the server");
			RPC.Connect(OnConnect);
		}

		void InitialiseDatabase(DatabaseConfiguration databaseConfiguration)
		{
			Database = new NpgsqlConnection("Server = " + databaseConfiguration.Host + "; Port = " + databaseConfiguration.Port + "; User Id = " + databaseConfiguration.Username + "; Database = " + databaseConfiguration.Database + "; Preload Reader = true;");
			try
			{
				Database.Open();
			}
			catch (Exception exception)
			{
				Console.WriteLine("Unable to connect to SQL server: " + exception);
				return;
			}
		}

		public void CloseDatabase()
		{
			Database.Close();
		}

		void WriteLine(string input, params object[] arguments)
		{
			Nil.Output.WriteLine(string.Format("{0} [{1}] {2}", Nil.Time.Timestamp(), RegionProfile.Abbreviation, input), arguments);
		}

		void SummonerMessage(string message, Summoner summoner, params object[] arguments)
		{
			WriteLine(string.Format("{0} ({1}): {2}", summoner.Name, summoner.AccountId, message), arguments);
		}

		void OnConnect(RPCConnectResult result)
		{
			if (result.Success())
			{
				WriteLine("Successfully connected to the server");
				//OnConnect must return so we can't use the current thread to execute region handler logic.
				//This is a limitation imposed by FluorineFX.
				(new Thread(Run)).Start();
			}
			else
				WriteLine(result.GetMessage());
		}

		void ProcessSummary(string mapEnum, string gameModeEnum, string target, Summoner summoner, List<PlayerStatSummary> summaries, bool forceNullRating = false)
		{
			foreach (var summary in summaries)
			{
				if (summary.playerStatSummaryType != target)
					continue;
				SQLCommand update = new SQLCommand("update summoner_rating set wins = :wins, losses = :losses, leaves = :leaves, current_rating = :current_rating, top_rating = :top_rating where summoner_id = :summoner_id and rating_map = cast(:rating_map as map_type) and game_mode = cast(:game_mode as game_mode_type)", Database);
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

				int rowsAffected = update.Execute();
				if (rowsAffected == 0)
				{
					//We're dealing with a new summoner rating entry, insert it
					SQLCommand insert = new SQLCommand("insert into summoner_rating (summoner_id, rating_map, game_mode, wins, losses, leaves, current_rating, top_rating) values (:summoner_id, cast(:rating_map as map_type), cast(:game_mode as game_mode_type), :wins, :losses, :leaves, :current_rating, :top_rating)", Database);
					insert.CopyParameters(update);
					insert.Execute();
					//SummonerMessage(string.Format("New rating for mode {0}", target), summoner);
				}
				else
				{
					//This rating was already in the database and was updated
					//SummonerMessage(string.Format("Updated rating for mode {0}", target), summoner);
				}
				break;
			}
		}

		void UpdateSummonerLastModifiedTimestamp(Summoner summoner)
		{
			SQLCommand timeUpdate = new SQLCommand(string.Format("update summoner set time_updated = {0} where id = :id", CurrentTimestamp()), Database);
			timeUpdate.Set("id", summoner.Id);
			timeUpdate.Execute();
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

		void InsertGameResult(Summoner summoner, int teamId, PlayerGameStats game, GameResult gameResult)
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

				//Items are stored as an SQL array
				"items",

				"kills",
				"deaths",
				"assists",

				"minion_kills",

				"gold",

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

				//Summoner's Rift/Twisted Treeline

				"neutral_minions_killed",

				"turrets_destroyed",
				"inhibitors_destroyed",

				//Dominon

				"nodes_neutralised",
				"node_neutralisation_assists",
				"nodes_captured",

				"victory_points",
				"objectives",

				"total_score",
				"objective_score",
				"combat_score",

				"rank",
			};

			string queryFields = GetGroupString(fields);
			string queryValues = GetPlaceholderString(fields);
			SQLCommand insert = new SQLCommand("insert into team_player ({0}) values ({1})", Database, queryFields, queryValues);
			insert.SetFieldNames(fields);

			insert.Set(teamId);
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
			insert.Set(NpgsqlDbType.Array | NpgsqlDbType.Integer, gameResult.Items);

			insert.Set(gameResult.Kills);
			insert.Set(gameResult.Deaths);
			insert.Set(gameResult.Assists);

			insert.Set(gameResult.MinionsKilled);

			insert.Set(gameResult.GoldEarned);

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

			//Summoner's Rift/Twisted Treeline

			insert.Set(gameResult.NeutralMinionsKilled);

			insert.Set(gameResult.TurretsDestroyed);
			insert.Set(gameResult.InhibitorsDestroyed);

			//Dominion

			insert.Set(gameResult.NodesNeutralised);
			insert.Set(gameResult.NodeNeutralisationAssists);
			insert.Set(gameResult.NodesCaptured);

			insert.Set(gameResult.VictoryPoints);
			insert.Set(gameResult.Objectives);

			insert.Set(gameResult.TotalScore);
			insert.Set(gameResult.ObjectiveScore);
			insert.Set(gameResult.CombatScore);

			insert.Set(gameResult.Rank);

			insert.Execute();
		}

		void UpdateSummonerGame(Summoner summoner, PlayerGameStats game, ref bool hasNormalElo, ref int normalElo)
		{
			if (game.queueType == "NONE")
			{
				//It's a tutorial game or something, don't store this
				return;
			}

			const int blueId = 100;
			const int purpleId = 200;

			bool isBlueTeam = game.teamId == blueId;

			//The update requires a transaction as multiple accounts might be querying data for the same game simultaneously
			NpgsqlTransaction transaction = Database.BeginTransaction();
			int summonerTeamId;
			GameResult gameResult = new GameResult(game);
			//At first we must determine if the game is already in the database
			SQLCommand check = new SQLCommand("select game_result.team1_id, game_result.team2_id, team.is_blue_team from game_result, team where game_result.game_id = :game_id and game_result.team1_id = team.id", Database);
			check.Set("game_id", game.gameId);
			var reader = check.ExecuteReader();
			if (reader.Read())
			{
				//The game is already in the database
				int team1Id = (int)reader[0];
				int team2Id = (int)reader[1];
				bool team1IsBlue = (bool)reader[2];
				if (isBlueTeam && team1IsBlue)
					summonerTeamId = team1Id;
				else
					summonerTeamId = team2Id;
				//Check if the game result for this player has already been stored
				SQLCommand gameCheck = new SQLCommand("select count(*) from team_player where (team_id = :team1_id or team_id = :team2_id) and summoner_id = :summoner_id", Database);
				gameCheck.Set("team1_id", team1Id);
				gameCheck.Set("team2_id", team2Id);
				gameCheck.Set("summoner_id", summoner.Id);
				long count = (long)gameCheck.ExecuteScalar();
				if (count > 0)
				{
					//The result of this game for this player has already been stored in the database, there is no work to be done
					transaction.Rollback();
					return;
				}
				//The game is already stored in the database but the results of this player were previously unknown
				//This means that this player must be removed from the list of unknown players for this game
				//I'm too lazy to figure out what team the player belongs to right now so let's just perform two deletions for now, one of which will fail
				int[] teamIds = {team1Id, team2Id};
				foreach(int teamId in teamIds)
				{
					SQLCommand delete = new SQLCommand("delete from missing_team_player where team_id = :team_id and account_id = :account_id", Database);
					delete.Set("team_id", teamId);
					delete.Set("account_id", summoner.AccountId);
					delete.Execute();
				}
			}
			else
			{
				//The game is not in the database yet
				//Need to create the team entries first
				SQLCommand newTeam = new SQLCommand("insert into team (is_blue_team) values (:is_blue_team)", Database);
				newTeam.Set("is_blue_team", NpgsqlDbType.Boolean, isBlueTeam);
				newTeam.Execute();
				int team1Id = GetInsertId("team");
				summonerTeamId = team1Id;
				newTeam.Set("is_blue_team", NpgsqlDbType.Boolean, !isBlueTeam);
				newTeam.Execute();
				int team2Id = GetInsertId("team");
				Dictionary<int, int> teamIdDictionary = new Dictionary<int, int>()
				{
					{game.teamId, team1Id},
					{isBlueTeam ? purpleId : blueId, team2Id},
				};
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
					//Autumn
					case 1:
					//No idea what 2 means
					case 2:
					//Winter
					case 6:
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
							if (mapEnum == "summoners_rift")
							{
								//Discovered a normal game of Summoner's Rift with Elo available, pass on this information to the outside function
								hasNormalElo = true;
								normalElo = game.rating + game.eloChange;
							}
							gameModeEnum = "normal";
							break;

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
							{
								transaction.Rollback();
								throw new Exception(string.Format("Unknown queue type in the match history of {0}: {1}", summoner.Name, game.queueType));
							}
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
				newGame.Execute();
				//We need to create a list of unknown players for this game so they can get updated in future if necessary
				//Otherwise it is unclear who participated in this game
				//Retrieving their stats at this point is too expensive and hence undesirable
				foreach (var player in game.fellowPlayers)
				{
					SQLCommand missingPlayer = new SQLCommand("insert into missing_team_player (team_id, champion_id, account_id) values (:team_id, :champion_id, :account_id)", Database);
					missingPlayer.Set("team_id", teamIdDictionary[player.teamId]);
					missingPlayer.Set("champion_id", player.championId);
					//It's called summonerId but it's really the account ID (I think)
					missingPlayer.Set("account_id", player.summonerId);
					missingPlayer.Execute();
				}
			}
			reader.Close();
			InsertGameResult(summoner, summonerTeamId, game, gameResult);
			transaction.Commit();
		}

		static int CompareGames(PlayerGameStats x, PlayerGameStats y)
		{
			return -x.gameId.CompareTo(y.gameId);
		}

		void UpdateSummonerGames(Summoner summoner, RecentGames recentGameData)
		{
			var recentGames = recentGameData.gameStatistics;
			recentGames.Sort(CompareGames);
			bool foundNormalElo = false;
			int currentNormalElo = 0;
			int currentTopElo = 0;
			foreach (var game in recentGames)
			{
				bool hasNormalElo = false;
				int normalElo = 0;
				UpdateSummonerGame(summoner, game, ref hasNormalElo, ref normalElo);
				if (hasNormalElo && !foundNormalElo)
				{
					currentNormalElo = normalElo;
					currentTopElo = Math.Max(currentTopElo, currentNormalElo);
					foundNormalElo = true;
				}
			}
			if (foundNormalElo)
			{
				//We discovered a new normal Elo value and must update the database accordingly
				SQLCommand update = new SQLCommand("update summoner_rating set current_rating = :current_rating, top_rating = greatest(top_rating, :top_rating)", Database);
				update.Set("current_rating", currentNormalElo);
				update.Set("top_rating", currentTopElo);
			}
		}

		void UpdateSummoner(Summoner summoner, bool isNewSummoner)
		{
			//Check for concurrency issues
			lock (ActiveAccountIds)
			{
				if (ActiveAccountIds.Contains(summoner.AccountId))
				{
					//This account is already being updated right now, do not proceed
					return;
				}
				else
				{
					//This account is currently not being updated by another worker, claim it for this worker
					ActiveAccountIds.Add(summoner.AccountId);
				}
			}

			SummonerMessage("Updating", summoner);

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

			//Release the lock on this account ID
			lock (ActiveAccountIds)
			{
				ActiveAccountIds.Remove(summoner.AccountId);
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
			//Attempt to retrieve an existing account ID to work with in order to avoid looking up the account ID again
			//Perform lower case comparison to account for misspelled versions of the name
			//LoL internally merges these to a mangled "internal name" for lookups anyways
			SQLCommand nameLookup = new SQLCommand("select id, account_id, summoner_name from summoner where region = cast(:region as region_type) and lower(summoner_name) = lower(:name)", Database);
			nameLookup.SetEnum("region", RegionProfile.RegionEnum);
			nameLookup.Set("name", summonerName);
			NpgsqlDataReader nameReader = nameLookup.ExecuteReader();
			if (nameReader.Read())
			{
				//The summoner already exists in the database
				int id = (int)nameReader[0];
				int accountId = (int)nameReader[1];
				string name = (string)nameReader[2];
				UpdateSummoner(new Summoner(name, id, accountId), false);
			}
			else
			{
				//We might be dealing with a new summoner
				PublicSummoner publicSummoner = RPC.GetSummonerByName(summonerName);
				if (publicSummoner == null)
				{
					WriteLine("No such summoner: {0}", summonerName);
					return;
				}

				SQLCommand check = new SQLCommand("select id from summoner where account_id = :account_id", Database);
				check.Set("account_id", publicSummoner.acctId);
				NpgsqlDataReader checkReader = check.ExecuteReader();
				if (checkReader.Read())
				{
					//We are dealing with an existing summoner even though the name lookup failed
					int id = (int)checkReader[0];
					UpdateSummoner(new Summoner(publicSummoner.name, id, publicSummoner.acctId), true);
				}
				else
				{
					//We are dealing with a new summoner
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

					string fieldsString = string.Format("region, {0}", GetGroupString(coreFields.Concat(extendedFields).ToList()));
					string placeholderString = GetPlaceholderString(coreFields);
					string valuesString = string.Format("cast(:region as region_type), {0}, {1}, {1}", placeholderString, CurrentTimestamp());
					string query = string.Format("insert into summoner ({0}) values ({1})", fieldsString, valuesString);

					SQLCommand newSummoner = new SQLCommand(query, Database);

					newSummoner.SetEnum("region", RegionProfile.RegionEnum);

					newSummoner.SetFieldNames(coreFields);
					newSummoner.Set(publicSummoner.acctId);
					newSummoner.Set(publicSummoner.summonerId);
					newSummoner.Set(publicSummoner.name);
					newSummoner.Set(publicSummoner.internalName);
					newSummoner.Set(publicSummoner.summonerLevel);
					newSummoner.Set(publicSummoner.profileIconId);

					newSummoner.Execute();

					int id = GetInsertId("summoner");
					UpdateSummoner(new Summoner(publicSummoner.name, id, publicSummoner.acctId), true);
				}
			}
			nameReader.Close();
		}

		int GetInsertId(string tableName)
		{
			SQLCommand currentValue = new SQLCommand(string.Format("select currval('{0}_id_seq')", tableName), Database);
			object result = currentValue.ExecuteScalar();
			long id = (long)result;
			return (int)id;
		}

		void Run()
		{
			while (true)
			{
				NpgsqlTransaction lookupTransaction = Database.BeginTransaction();
				SQLCommand getJob = new SQLCommand("select id, summoner_name from lookup_job where region = cast(:region as region_type) order by priority desc, time_added limit 1", Database);
				getJob.SetEnum("region", RegionProfile.RegionEnum);
				NpgsqlDataReader reader = getJob.ExecuteReader();
				bool success = reader.Read();

				string summonerName = null;

				if (success)
				{
					int id = (int)reader[0];
					summonerName = (string)reader[1];

					//Delete entry
					SQLCommand deleteJob = new SQLCommand("delete from lookup_job where id = :id", Database);
					deleteJob.Set("id", id);
					deleteJob.Execute();
				}

				reader.Close();
				lookupTransaction.Commit();

				if (success)
					UpdateSummonerByName(summonerName);
				else
				{
					WriteLine("No jobs available");
					//Should wait for an event here really but can't provide any code until there is some user driven interface for the SQL backend
					break;
				}
			}
		}
	}
}
