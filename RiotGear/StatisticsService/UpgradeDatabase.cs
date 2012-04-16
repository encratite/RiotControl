using System;
using System.IO;
using System.Data.Common;

namespace RiotGear
{
	public partial class StatisticsService
	{

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
				using (var transaction = connection.BeginTransaction())
				{
					//Rename the old table to a temporary name
					using (var renameTable = new DatabaseCommand("alter table unknown_player rename to broken_unknown_player", connection))
						renameTable.Execute();
					//Create the new table
					using (var createTable = new DatabaseCommand(Properties.Resources.CreateTableUnknownPlayer, connection))
						createTable.Execute();
					//Insert the data from the old table into the new table
					using (var insert = new DatabaseCommand("insert into unknown_player (team_id, champion_id, summoner_id) select team_id, champion_id, account_id from broken_unknown_player", connection))
						insert.Execute();
					//Remove the old table
					using (var dropTable = new DatabaseCommand("drop table broken_unknown_player", connection))
						dropTable.Execute();
					//Commit the transaction
					transaction.Commit();
					//Vacuum
					using (var vacuum = new DatabaseCommand("vacuum", connection))
						vacuum.Execute();
				}
				WriteLine("Upgrade succeeded.");
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
				WriteLine("This database is affected by the pre-348 player.game_id bug. Attempting to upgrade it");

				using (var transaction = connection.BeginTransaction())
				{
					//Rename the old table to a temporary name
					using (var renameTable = new DatabaseCommand("alter table player rename to broken_player", connection))
						renameTable.Execute();
					//Create the new table
					using (var createTable = new DatabaseCommand(Properties.Resources.CreateTablePlayer, connection))
						createTable.Execute();
					//Insert the data from the old table into the new table
					string fieldString = Properties.Resources.PlayerFields.Replace("\n", " ");
					using (var insert = new DatabaseCommand(string.Format("insert into player ({0}) select {0} from broken_player", fieldString), connection))
						insert.Execute();
					//Remove the old table
					using (var dropTable = new DatabaseCommand("drop table broken_player", connection))
						dropTable.Execute();
					//Commit the transaction
					transaction.Commit();
					//Vacuum
					using (var vacuum = new DatabaseCommand("vacuum", connection))
						vacuum.Execute();
				}
				WriteLine("Upgrade succeeded.");
			}
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
				}
			}
		}
	}
}
