using System;

namespace RiotGear
{
	public partial class StatisticsService
	{
		//This function is required to upgrade old database formats to newer ones
		void UpgradeDatabase()
		{
			using (var connection = Provider.GetConnection())
			{
				//This legacy upgrade is only required for old SQLite databases
				if (Provider.IsSQLite())
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
						const string createTableQuery = "create table unknown_player(team_id integer not null, champion_id integer not null, summoner_id integer not null, foreign key (team_id) references team(id))";

						WriteLine("This database is affected by the pre-r248 unknown_player bug. Attempting to upgrade it.");
						using (var transaction = connection.BeginTransaction())
						{
							//Rename the old table to a temporary name
							using (var renameTable = new DatabaseCommand("alter table unknown_player rename to broken_unknown_player", connection))
								renameTable.Execute();
							//Create the new table
							using (var createTable = new DatabaseCommand(createTableQuery, connection))
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
			}
		}
	}
}
