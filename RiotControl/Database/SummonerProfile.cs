using System.Collections.Generic;

namespace RiotControl
{
	public class SummonerProfile
	{
		//Not the most elegant name for this property but it is more convenient for the JSON reflection
		public Summoner Summoner;
		public List<SummonerRating> Ratings;
		//This might look confusing, but it's one set of statistics for every ranked season
		//The index of zero refers to the current season
		public List<List<SummonerRankedStatistics>> RankedStatistics;
		public List<AggregatedChampionStatistics> TwistedTreelineStatistics;
		public List<AggregatedChampionStatistics> SummonersRiftStatistics;
		public List<AggregatedChampionStatistics> DominionStatistics;

		public SummonerProfile(Summoner summoner, List<SummonerRating> ratings, List<List<SummonerRankedStatistics>> rankedStatistics, List<AggregatedChampionStatistics> twistedTreelineStatistics, List<AggregatedChampionStatistics> summonersRiftStatistics, List<AggregatedChampionStatistics> dominionStatistics)
		{
			Summoner = summoner;
			Ratings = ratings;
			RankedStatistics = rankedStatistics;
			TwistedTreelineStatistics = twistedTreelineStatistics;
			SummonersRiftStatistics = summonersRiftStatistics;
			DominionStatistics = dominionStatistics;
		}
	}
}
