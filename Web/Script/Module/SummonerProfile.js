function convertStatistics(statistics)
{
    if(statistics.length === 0)
        return [];
    var summary = new BasicStatistics();
    var output = [summary];
    statistics.forEach(function(currentStatistics) {
        var currentBasicStatistics = new BasicStatistics(currentStatistics);
        summary.add(currentBasicStatistics);
        output.push(currentBasicStatistics);
    });
    return output;
}

function getStatisticsContainer(description, containerName, statistics)
{
    sortStatistics(statistics);
    var container = diverse(getStatisticsTable(description, statistics, containerName));
    container.id = containerName;
    initialiseSortableContainer(container);
    return container;
}

function viewSummonerProfile(region, accountId)
{
    apiGetSummonerProfile(region, accountId, function (response) { onGetSummonerProfile(response, region, accountId); } );
}

function getSeasonStatistics(statistics, season)
{
    var summary = new RankedStatistics();
    var rankedStatistics = [];
    //Only examine index 0, which is where the current season is being held
    var currentSeasonStatistics = statistics.RankedStatistics[season];
    if(currentSeasonStatistics.length > 0)
        rankedStatistics.push(summary);
    currentSeasonStatistics.forEach(function(currentStatistics) {
        var currentRankedStatistics =  new RankedStatistics(currentStatistics);
        rankedStatistics.push(currentRankedStatistics);
        summary.add(currentRankedStatistics);
    });
    return getStatisticsContainer('Ranked Statistics (' + (season == 0 ? 'current season' : 'season ' + season) + ')', 'rankedStatistics' + season, rankedStatistics);
}

function renderSummonerProfile(region, summoner, statistics)
{
    setTitle(summoner.SummonerName);

    var overview = getSummonerOverview(summoner, statistics);
    var ratings = getRatingTable(region, summoner, statistics);

    var items = [
        overview,
        ratings,
    ];

    items.push(getSeasonStatistics(statistics, 0));

    for(var season = statistics.RankedStatistics.length - 1; season >= 1; season--)
        items.push(getSeasonStatistics(statistics, season));

    items = items.concat([
        getStatisticsContainer('Unranked Twisted Treeline Statistics', 'twistedTreelineStatistics', convertStatistics(statistics.TwistedTreelineStatistics)),
        getStatisticsContainer("Unranked Summoner's Rift Statistics", 'summonersRiftStatistics', convertStatistics(statistics.SummonersRiftStatistics)),
        getStatisticsContainer('Unranked Dominion Statistics', 'dominionStatistics', convertStatistics(statistics.DominionStatistics))
    ]);

    renderWithSearchForm(items);
}