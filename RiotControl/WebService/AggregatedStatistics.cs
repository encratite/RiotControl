using System.Collections.Generic;

using Npgsql;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		string GetAggregatedChampionStatistics(string caption, string containerName, List<AggregatedChampionStatistics> statistics)
		{
			string container = Markup.Diverse("", id: containerName);

			string statisticsVariable = string.Format("{0}Statistics", containerName);

			string script = string.Format("var {0} =\n", statisticsVariable);
			script += "[\n";
			foreach (var champion in statistics)
			{
				string[] fields =
				{
					GetJavaScriptString(champion.ChampionName),
					
					champion.Wins.ToString(),
					champion.Losses.ToString(),

					champion.Kills.ToString(),
					champion.Deaths.ToString(),
					champion.Assists.ToString(),

					champion.MinionKills.ToString(),
					champion.Gold.ToString(),
				};

				script += string.Format("new BasicStatistics({0}),\n", string.Join(", ", fields));
			}
			script += "];\n";

			script += string.Format("initialiseContainer({0}, {1}, {2});", GetJavaScriptString(caption), GetJavaScriptString(containerName), statisticsVariable);
			script = Markup.InlineScript(script);

			string output = container + script;

			return output;
		}

		string GetAggregatedChampionStatistics(Summoner summoner)
		{
			string output = GetAggregatedChampionStatistics("Normal Summoner's Rift Statistics", "summonersRiftNormal", summoner.SummonersRiftNormalStatistics);
			output += GetAggregatedChampionStatistics("Normal Dominion Statistics", "dominionNormal", summoner.DominionNormalStatistics);
			return output;
		}

		void LoadAggregatedChampionStatistics(Summoner summoner, NpgsqlConnection database)
		{
			summoner.SummonersRiftNormalStatistics = LoadAggregatedChampionStatistics(summoner, MapType.SummonersRift, GameModeType.Normal, database);
			summoner.DominionNormalStatistics = LoadAggregatedChampionStatistics(summoner, MapType.Dominion, GameModeType.Normal, database);
		}
	}
}
