using System;
using System.Data.Common;

using Nil;

using com.riotgames.platform.gameclient.domain;

namespace RiotControl
{
	partial class Worker
	{
		void UpdateRunes(Summoner summoner, AllPublicSummonerDataDTO publicSummonerData, DbConnection connection)
		{
			//Remove old rune pages from the database first
			using (var delete = Command("delete from rune_page where summoner_id = :summoner_id", connection))
			{
				delete.Set("summoner_id", summoner.Id);
				delete.Execute();
			}

			foreach (var page in publicSummonerData.spellBook.bookPages)
			{
				string[] pageFields =
				{
					"summoner_id",
					"name",
					"is_current_rune_page",
					"time_created",
				};

				using (var insert = Command("insert into rune_page ({0}) values ({1})", connection, GetGroupString(pageFields), GetPlaceholderString(pageFields)))
				{
					insert.SetFieldNames(pageFields);

					insert.Set(summoner.Id);
					insert.Set(page.name);
					insert.Set(page.current);
					insert.Set(page.createDate.ToUnixTime());

					insert.Execute();
				}

				int runePageId = GetInsertId(connection);

				string[] runeFields =
				{
					"rune_page_id",
					"rune_slot",
					"rune",
				};

				foreach (var rune in page.slotEntries)
				{
					using (var insert = Command("insert into rune_slot ({0}) values ({1})", connection, GetGroupString(runeFields), GetPlaceholderString(runeFields)))
					{
						insert.SetFieldNames(runeFields);

						insert.Set(runePageId);
						insert.Set(rune.runeSlotId);
						insert.Set(rune.runeId);

						insert.Execute();
					}
				}
			}
				
		}
	}
}
