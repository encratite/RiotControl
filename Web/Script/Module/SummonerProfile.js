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

function renderSummonerProfile(summoner, statistics)
{
    setTitle(summoner.SummonerName);

    var searchForm = getSearchForm(null);
    var searchFormContainer = diverse();
    searchFormContainer.id = 'searchForm';
    searchFormContainer.add(searchForm);

    var overview = getSummonerOverview(summoner, statistics);
    var ratings = getRatingTable(statistics);

    var summary = new RankedStatistics();
    var rankedStatistics = [];
    //Only examine index 0, which is where the current season is being held
    var currentSeasonStatistics = statistics.RankedStatistics[0];
    if(currentSeasonStatistics.length > 0)
        rankedStatistics.push(summary);
    currentSeasonStatistics.forEach(function(currentStatistics) {
        var currentRankedStatistics =  new RankedStatistics(currentStatistics);
        rankedStatistics.push(currentRankedStatistics);
        summary.add(currentRankedStatistics);
    });

    var items = [];

    if(system.privileged)
        items.push(searchFormContainer);

    items = items.concat(
        [
            overview,
            ratings,
            getStatisticsContainer('Ranked Statistics', 'rankedStatistics', rankedStatistics),
            getStatisticsContainer('Unranked Twisted Treeline Statistics', 'twistedTreelineStatistics', convertStatistics(statistics.TwistedTreelineStatistics)),
            getStatisticsContainer("Unranked Summoner's Rift Statistics", 'summonersRiftStatistics', convertStatistics(statistics.SummonersRiftStatistics)),
            getStatisticsContainer('Unranked Dominion Statistics', 'dominionStatistics', convertStatistics(statistics.DominionStatistics))
        ]
    );

    render(items);
}