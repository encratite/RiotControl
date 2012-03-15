using System.Linq;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		string GetRankedStatistics(Summoner summoner)
		{
			const string caption = "Ranked Statistics";
			const string containerName = "rankedStatistics";
			string table = Markup.Diverse("", id: containerName);

			string inline = "var rankedStatistics =\n[\n";
			foreach (var champion in summoner.RankedStatistics)
			{
				int[] fields =
				{
					champion.Wins,
					champion.Losses,

					champion.Kills,
					champion.Deaths,
					champion.Assists,

					champion.MinionKills,

					champion.Gold,

					champion.TurretsDestroyed,

					champion.DamageDealt,
					champion.PhysicalDamageDealt,
					champion.MagicalDamageDealt,

					champion.DamageTaken,

					champion.DoubleKills,
					champion.TripleKills,
					champion.QuadraKills,
					champion.PentaKills,

					champion.TimeSpentDead,

					champion.MaximumKills,
					champion.MaximumDeaths,
				};
				var stringFields = from x in fields select x.ToString();
				inline += "new RankedStatistics(";
				inline += GetJavaScriptString(champion.ChampionName) + ", ";
				inline += string.Join(", ", stringFields);
				inline += "),\n";
			}
			inline += "];\n";
			inline += string.Format("initialiseContainer({0}, {1}, rankedStatistics);", GetJavaScriptString(caption), GetJavaScriptString(containerName));

			string inlineScript = Markup.InlineScript(inline);

			string output = table + inlineScript;

			return output;
		}
	}
}
