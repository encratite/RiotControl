using System.Collections.Generic;

namespace RiotControl
{
	public class SummonerProfile
	{
		//Not the most elegant name for this property but it is more convenient for the JSON reflection
		public Summoner Summoner;
		public List<SummonerRating> Ratings;
		public List<SummonerRankedStatistics> RankedStatistics;
		public List<AggregatedChampionStatistics> UnrankedStatistics;
		public List<AggregatedChampionStatistics> DominionStatistics;

		public SummonerProfile(Summoner summoner, List<SummonerRating> ratings, List<SummonerRankedStatistics> rankedStatistics, List<AggregatedChampionStatistics> unrankedStatistics, List<AggregatedChampionStatistics> dominionStatistics)
		{
			Summoner = summoner;
			Ratings = ratings;
			RankedStatistics = rankedStatistics;
			UnrankedStatistics = unrankedStatistics;
			DominionStatistics = dominionStatistics;
		}
	}
}
