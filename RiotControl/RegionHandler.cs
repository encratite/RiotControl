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
			Nil.Output.WriteLine(Nil.Time.Timestamp() + " [" + RegionProfile.Abbreviation + "] " + input, arguments);
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

		void UpdateSummoner(int id, int accountId)
		{
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

		void UpdateSummonerByName(string summonerName)
		{
			NpgsqlCommand nameLookup = new NpgsqlCommand("select id, account_id from summoner where summoner_name = :name", Database);
			nameLookup.Parameters.Add(new NpgsqlParameter("name", NpgsqlDbType.Text));
			NpgsqlDataReader reader = nameLookup.ExecuteReader();
			if (reader.Read())
			{
				//The summoner already exists in the database
				int id = (int)reader[0];
				int accountId = (int)reader[1];
				UpdateSummoner(id, accountId);
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
					"region",
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

				string fieldsString = GetGroupString(coreFields.Concat(extendedFields).ToList());
				string placeholderString = GetPlaceholderString(coreFields);
				string currentTimestamp = Zulufy("current_timestamp");
				string valuesString = string.Format("{0}, time_created = {1}, time_updated = {1}", placeholderString, currentTimestamp);

				NpgsqlCommand newSummoner = new NpgsqlCommand(string.Format("insert into summoner ({0}) values ({1})", fieldsString, valuesString), Database);

				newSummoner.Parameters.Set(field, NpgsqlDbType.Varchar, RegionProfile.RegionEnum);
				newSummoner.Parameters.Set(field, summoner.acctId);
				newSummoner.Parameters.Set(field, summoner.summonerId);
				newSummoner.Parameters.Set(field, summonerName);
				newSummoner.Parameters.Set(field, summoner.internalName);
				newSummoner.Parameters.Set(field, summoner.summonerLevel);
				newSummoner.Parameters.Set(field, summoner.profileIconId);

				newSummoner.ExecuteNonQuery();

				int id = InsertId("summoner");
				UpdateSummoner(id, summoner.acctId);
			}
		}

		int InsertId(string tableName)
		{
			NpgsqlCommand currentValue = new NpgsqlCommand(string.Format("select currval('{0}_id_seq')", tableName), Database);
			int id = (int)currentValue.ExecuteScalar();
			return id;
		}

		void Run()
		{
			NpgsqlCommand getJob = new NpgsqlCommand("select id, summoner_name from lookup_job where region = cast(:region as region_type) order by priority desc, time_added limit 1", Database);
			getJob.Parameters.Add(new NpgsqlParameter("region", NpgsqlDbType.Varchar));
			getJob.Parameters[0].Value = RegionProfile.RegionEnum;

			NpgsqlCommand deleteJob = new NpgsqlCommand("delete from lookup_job where id = :id", Database);
			deleteJob.Parameters.Add(new NpgsqlParameter("id", NpgsqlDbType.Integer));

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
			}
		}
	}
}
