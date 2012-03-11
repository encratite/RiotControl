using System;
using System.Collections.Generic;

using Npgsql;
using NpgsqlTypes;

using LibOfLegends;

using com.riotgames.platform.statistics;

namespace RiotControl
{
	partial class Worker
	{
		double GetTimestamp(DateTime input)
		{
			DateTime epoch = new DateTime(1970, 1, 1).ToLocalTime();
			TimeSpan difference = input - epoch;
			return difference.TotalSeconds;
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
			SQLCommand check = Command("select game_result.team1_id, game_result.team2_id, team.is_blue_team from game_result, team where game_result.game_id = :game_id and game_result.team1_id = team.id");
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
				SQLCommand gameCheck = Command("select count(*) from team_player where (team_id = :team1_id or team_id = :team2_id) and summoner_id = :summoner_id");
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
				int[] teamIds = { team1Id, team2Id };
				foreach (int teamId in teamIds)
				{
					SQLCommand delete = Command("delete from missing_team_player where team_id = :team_id and account_id = :account_id");
					delete.Set("team_id", teamId);
					delete.Set("account_id", summoner.AccountId);
					delete.Execute();
				}
			}
			else
			{
				//The game is not in the database yet
				//Need to create the team entries first
				SQLCommand newTeam = Command("insert into team (is_blue_team) values (:is_blue_team)");
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
						case "RANKED_PREMADE_3x3":
						case "RANKED_PREMADE_5x5":
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
				SQLCommand newGame = Command("insert into game_result ({0}) values ({1})", queryFields, queryValues);
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
					SQLCommand missingPlayer = Command("insert into missing_team_player (team_id, champion_id, account_id) values (:team_id, :champion_id, :account_id)");
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
	}
}
