using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;

using Nil;

namespace RiotGear
{
	public partial class StatisticsService
	{
		string GetTableFieldsFromCreateTableQuery(string query)
		{
			var fields = new List<string>();
			foreach (string i in query.Tokenise("\n"))
			{
				string line = i.Trim();
				if(line.Length > 0 && line[line.Length - 1] == ',')
					line = line.Substring(0, line.Length - 1);
				var tokens = line.Tokenise(" ");
				if (tokens.Count >= 2)
				{
					string fieldType = tokens[1];
					if (fieldType == "integer" || fieldType == "text")
						fields.Add(tokens[0]);
				}
			}
			return string.Join(", ", fields);
		}

		List<string> GetIndexNames(string query)
		{
			var indexNames = new List<string>();
			foreach (string line in query.Tokenise("\n"))
			{
				var tokens = line.Tokenise(" ");
				if (tokens.Count > 3 && tokens[1] == "index")
					indexNames.Add(tokens[2]);
			}
			return indexNames;
		}

		void UnknownPlayerUpgrade(DbConnection connection)
		{
			//This is required for the r248 fix in the unknown_player table
			//The field "account_id" had to be renamed to "summoner_id" in that version
			//First check if this database is actually affected by that bug
			bool isAffected = false;
			using (var pragma = new DatabaseCommand("pragma table_info(unknown_player)", connection))
			{
				using (var reader = pragma.ExecuteReader())
				{
					while (reader.Read())
					{
						string name = (string)reader.Get("name");
						if (name == "account_id")
						{
							isAffected = true;
							break;
						}
					}
				}
			}
			if (isAffected)
			{
				WriteLine("This database is affected by the pre-r248 unknown_player bug. Attempting to upgrade it.");
				CopyTableTransaction(connection, "unknown_player", Properties.Resources.CreateTableUnknownPlayer);
			}
		}

		void GameIdUpgrade(DbConnection connection)
		{
			//player.game_id was nullable prior to r348, should have been not null all along
			bool isAffected = false;
			using (var pragma = new DatabaseCommand("pragma table_info(player)", connection))
			{
				using (var reader = pragma.ExecuteReader())
				{
					while (reader.Read())
					{
						string name = (string)reader.Get("name");
						if (name != "game_id")
							continue;
						object notNullObject = reader.Get("notnull");
						bool isNotNull = (int)(long)notNullObject == 1;
						if (!isNotNull)
						{
							isAffected = true;
							break;
						}
					}
				}
			}

			if (isAffected)
			{
				WriteLine("This database is affected by the pre-348 player.game_id bug. Attempting to upgrade it.");
				CopyTableTransaction(connection, "player", Properties.Resources.CreateTablePlayer);
			}
		}

		void AccountIdUpgrade(DbConnection connection)
		{
			//In pre-r375 databases the account_id was marked as unique even though it shouldn't have been
			bool isAffected = false;
			using (var pragma = new DatabaseCommand("select sql from sqlite_master where tbl_name = 'summoner'", connection))
			{
				using (var reader = pragma.ExecuteReader())
				{
					if (reader.Read())
					{
						string query = reader.String();
						if (query.IndexOf("account_id integer unique not null") >= 0)
							isAffected = true;
					}
					else
						throw new Exception("Unable to locate summoner table for upgrade check");
				}
			}

			if (isAffected)
			{
				WriteLine("This database is affected by the pre-375 summoner.account_id bug. Attempting to upgrade it.");
				using (var transaction = connection.BeginTransaction())
				{
					CopyTable(connection, "summoner", Properties.Resources.CreateTableSummoner);
					CopyTable(connection, "summoner_rating", Properties.Resources.CreateTableSummonerRating);
					CopyTable(connection, "summoner_ranked_statistics", Properties.Resources.CreateTableSummonerRankedStatistics);
					CopyTable(connection, "player", Properties.Resources.CreateTablePlayer);
					CopyTable(connection, "rune_page", Properties.Resources.CreateTableRunePage);
					CopyTable(connection, "rune_slot", Properties.Resources.CreateTableRuneSlot);
					DropOldTable(connection, "rune_slot");
					DropOldTable(connection, "rune_page");
					DropOldTable(connection, "player");
					DropOldTable(connection, "summoner_ranked_statistics");
					DropOldTable(connection, "summoner_rating");
					DropOldTable(connection, "summoner");
					transaction.Commit();
				}
				Vacuum(connection);
				WriteLine("Upgrade succeeded.");
			}
		}

		void CopyTable(DbConnection connection, string tableName, string createTableQuery)
		{
			//Rename the old table to a temporary name
			using (var renameTable = new DatabaseCommand(string.Format("alter table {0} rename to broken_{0}", tableName), connection))
				renameTable.Execute();
			//Remove indices
			var indexNames = GetIndexNames(createTableQuery);
			foreach (var index in indexNames)
			{
				try
				{
					using (var dropIndex = new DatabaseCommand(string.Format("drop index {0}", index), connection))
						dropIndex.Execute();
				}
				catch (Exception exception)
				{
					WriteLine("Warning - failed to remove index {0} while performing an upgrade: {1}", index, exception.Message);
				}
			}
			//Create the new table
			using (var createTable = new DatabaseCommand(createTableQuery, connection))
				createTable.Execute();
			//Insert the data from the old table into the new table
			string tableFields = GetTableFieldsFromCreateTableQuery(createTableQuery);
			string fieldString = tableFields.Replace("\n", " ");
			using (var insert = new DatabaseCommand(string.Format("insert into {0} ({1}) select {1} from broken_{0}", tableName, fieldString), connection))
				insert.Execute();
		}

		void DropOldTable(DbConnection connection, string tableName)
		{
			using (var dropTable = new DatabaseCommand(string.Format("drop table broken_{0}", tableName), connection))
				dropTable.Execute();
		}

		void Vacuum(DbConnection connection)
		{
			using (var vacuum = new DatabaseCommand("vacuum", connection))
				vacuum.Execute();
		}

		void CopyTableTransaction(DbConnection connection, string tableName, string createTableQuery)
		{
			using (var transaction = connection.BeginTransaction())
			{
				CopyTable(connection, tableName, createTableQuery);
				DropOldTable(connection, tableName);
				transaction.Commit();
			}
			Vacuum(connection);
			WriteLine("Upgrade succeeded.");
		}

		//This function is required to upgrade old database formats to newer ones
		void UpgradeDatabase()
		{
			using (var connection = Provider.GetConnection())
			{
				//This legacy upgrade is only required for old SQLite databases
				if (Provider.IsSQLite())
				{
					UnknownPlayerUpgrade(connection);
					GameIdUpgrade(connection);
					AccountIdUpgrade(connection);
				}
			}
		}
	}
}
