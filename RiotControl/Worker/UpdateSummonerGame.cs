using System;
using System.Collections.Generic;

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

		void UpdateSummonerGame(SummonerDescription summoner, PlayerGameStats game)
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
			DatabaseCommand check = Command("select id, blue_team_id, purple_team_id, purple_team_won from game where game.game_id = :game_id");
			check.Set("game_id", game.gameId);
			using (var reader = check.ExecuteReader())
			{
				if (reader.Read())
				{
					//The game is already in the database
					gameId = reader.Integer();
					int purpleTeamId = reader.Integer();
					int blueTeamId = reader.Integer();
					bool PurpleTeamWon = reader.Boolean();
					if (isBlueTeam)
						summonerTeamId = blueTeamId;
					else
						summonerTeamId = purpleTeamId;
					//Check if the game result for this player has already been stored
					DatabaseCommand gameCheck = Command("select count(*) from player where (team_id = :blue_team_id or team_id = :purple_team_id) and summoner_id = :summoner_id");
					gameCheck.Set("blue_team_id", blueTeamId);
					gameCheck.Set("purple_team_id", purpleTeamId);
					gameCheck.Set("summoner_id", summoner.Id);
					long count = (long)gameCheck.ExecuteScalar();
					if (count > 0)
					{
						//The result of this game for this player has already been stored in the database, there is no work to be done
						return;
					}
					//The game is already stored in the database but the results of this player were previously unknown
					//This means that this player must be removed from the list of unknown players for this game
					DatabaseCommand delete = Command("delete from missing_team_player where team_id = :team_id and account_id = :account_id");
					delete.Set("team_id", summonerTeamId);
					delete.Set("account_id", summoner.AccountId);
					delete.Execute();
				}
				else
				{
					//The game is not in the database yet
					//Need to create the team entries first
					DatabaseCommand newTeam = Command("insert into team default values");
					newTeam.Execute();
					int blueTeamId = GetInsertId();
					newTeam.Execute();
					int purpleTeamId = GetInsertId();
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
							throw new Exception(string.Format("Unknown game map ID in the match history of {0}: {1}", summoner.Name, game.gameMapId));
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
								throw new Exception(string.Format("Unknown queue type in the match history of {0}: {1}", summoner.Name, game.queueType));
						}
					}
					List<string> fields = new List<string>()
					{
						"game_id",
						"map",
						"game_mode",
						"time",
						"blue_team_id",
						"purple_team_id",
						"blue_team_won",
					};
					DatabaseCommand newGame = Command("insert into game ({0}) values ({1})", GetGroupString(fields), GetPlaceholderString(fields));
					newGame.SetFieldNames(fields);
					newGame.Set(game.gameId);
					newGame.Set(map);
					newGame.Set(gameMode);
					newGame.Set(GetTimestamp(game.createDate));
					newGame.Set(blueTeamId);
					newGame.Set(purpleTeamId);
					newGame.Set(gameResult.Win && isBlueTeam);
					newGame.Execute();
					gameId = GetInsertId();
					//We need to create a list of unknown players for this game so they can get updated in future if necessary
					//Otherwise it is unclear who participated in this game
					//Retrieving their stats at this point is too expensive and hence undesirable
					foreach (var player in game.fellowPlayers)
					{
						DatabaseCommand missingPlayer = Command("insert into missing_team_player (team_id, champion_id, account_id) values (:team_id, :champion_id, :account_id)");
						missingPlayer.Set("team_id", player.teamId == blueId ? blueTeamId : purpleTeamId);
						missingPlayer.Set("champion_id", player.championId);
						//It's called summonerId but it's really the account ID (I think)
						missingPlayer.Set("account_id", player.summonerId);
						missingPlayer.Execute();
					}
				}
			}
			InsertGameResult(summoner, gameId, summonerTeamId, game, gameResult);
		}
	}
}
