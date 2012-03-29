using System;
using System.Collections.Generic;
using System.Data.Common;

using Nil;
using LibOfLegends;

using com.riotgames.platform.statistics;

namespace RiotControl
{
	partial class Worker
	{
		static string[] NewGameFields =
		{
			"game_id",
			"map",
			"game_mode",
			"time",
			"blue_team_id",
			"purple_team_id",
			"blue_team_won",
		};

		void UpdateSummonerGame(Summoner summoner, PlayerGameStats game, DbConnection connection)
		{
			//Don't store tutorial games
			if (game.gameMode == "TUTORIAL" || game.gameType == "TUTORIAL_GAME")
				return;

			const int blueId = 100;
			//const int purpleId = 200;

			bool isBlueTeam = game.teamId == blueId;

			int gameId;
			int summonerTeamId;
			GameResult gameResult = new GameResult(game);
			//At first we must determine if the game is already in the database
			using (var check = Command("select id, blue_team_id, purple_team_id from game where game.game_id = :game_id", connection))
			{
				check.Set("game_id", game.gameId);
				using (var reader = check.ExecuteReader())
				{
					if (reader.Read())
					{
						//The game is already in the database
						gameId = reader.Integer();
						int purpleTeamId = reader.Integer();
						int blueTeamId = reader.Integer();
						if (isBlueTeam)
							summonerTeamId = blueTeamId;
						else
							summonerTeamId = purpleTeamId;
						//Check if the game result for this player has already been stored
						using (var gameCheck = Command("select count(*) from player where (team_id = :blue_team_id or team_id = :purple_team_id) and summoner_id = :summoner_id", connection))
						{
							gameCheck.Set("blue_team_id", blueTeamId);
							gameCheck.Set("purple_team_id", purpleTeamId);
							gameCheck.Set("summoner_id", summoner.Id);
							long count = (long)gameCheck.ExecuteScalar();
							if (count > 0)
							{
								//The result of this game for this player has already been stored in the database, there is no work to be done
								return;
							}
						}
						//The game is already stored in the database but the results of this player were previously unknown
						//This means that this player must be removed from the list of unknown players for this game
						using (var delete = Command("delete from unknown_player where team_id = :team_id and account_id = :account_id", connection))
						{
							delete.Set("team_id", summonerTeamId);
							delete.Set("account_id", summoner.AccountId);
							delete.Execute();
						}
					}
					else
					{
						//The game is not in the database yet
						//Need to create the team entries first
						using (var newTeam = Command("insert into team default values", connection))
						{
							newTeam.Execute();
							int blueTeamId = GetInsertId(connection);
							newTeam.Execute();
							int purpleTeamId = GetInsertId(connection);
							summonerTeamId = isBlueTeam ? blueTeamId : purpleTeamId;
							MapType map;
							GameModeType gameMode;
							switch (game.gameMapId)
							{
								//Autumn
								case 1:
								//No idea what 2 means
								case 2:
								//Not sure either, encountered this in some games from 4 months ago on an inactive account
								case 3:
								//Winter
								case 6:
									map = MapType.SummonersRift;
									break;

								case 4:
									map = MapType.TwistedTreeline;
									break;

								case 8:
									map = MapType.Dominion;
									break;

								default:
									throw new Exception(string.Format("Unknown game map ID in the match history of {0}: {1}", summoner.SummonerName, game.gameMapId));
							}
							if (game.gameType == "PRACTICE_GAME")
								gameMode = GameModeType.Custom;
							else
							{
								switch (game.queueType)
								{
									case "RANKED_TEAM_3x3":
									case "RANKED_TEAM_5x5":
									case "RANKED_PREMADE_3x3":
									case "RANKED_PREMADE_5x5":
										gameMode = GameModeType.Premade;
										break;

									case "NORMAL":
									case "ODIN_UNRANKED":
										gameMode = GameModeType.Normal;
										break;

									case "RANKED_SOLO_5x5":
										gameMode = GameModeType.Solo;
										break;

									case "BOT":
										gameMode = GameModeType.Bot;
										break;

									default:
										throw new Exception(string.Format("Unknown queue type in the match history of {0}: {1}", summoner.SummonerName, game.queueType));
								}
							}

							using (var newGame = Command("insert into game ({0}) values ({1})", connection, GetGroupString(NewGameFields), GetPlaceholderString(NewGameFields)))
							{
								newGame.SetFieldNames(NewGameFields);
								newGame.Set(game.gameId);
								newGame.Set(map);
								newGame.Set(gameMode);
								newGame.Set(game.createDate.ToUnixTime());
								newGame.Set(blueTeamId);
								newGame.Set(purpleTeamId);
								newGame.Set(gameResult.Win == isBlueTeam);
								newGame.Execute();
								gameId = GetInsertId(connection);
								//We need to create a list of unknown players for this game so they can get updated in future if necessary
								//Otherwise it is unclear who participated in this game
								//Retrieving their stats at this point is too expensive and hence undesirable
								foreach (var player in game.fellowPlayers)
								{
									using (var missingPlayer = Command("insert into unknown_player (team_id, champion_id, account_id) values (:team_id, :champion_id, :account_id)", connection))
									{
										missingPlayer.Set("team_id", player.teamId == blueId ? blueTeamId : purpleTeamId);
										missingPlayer.Set("champion_id", player.championId);
										//It's called summonerId but it's really the account ID (I think)
										missingPlayer.Set("account_id", player.summonerId);
										missingPlayer.Execute();
									}
								}
							}
						}
					}
				}
			}
			InsertGameResult(summoner, gameId, summonerTeamId, game, gameResult, connection);
		}
	}
}
