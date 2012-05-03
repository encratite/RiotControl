function viewGraph(region, accountId, additionalArguments)
{
    if(additionalArguments === undefined || additionalArguments.length < 2)
        throw 'Missing arguments';

    apiGetSummonerProfile(region, accountId, function (response) { onGetSummonerProfileForGraph(response, region, additionalArguments); } );
}

function getRating(statistics, map, gameMode)
{
    var output = null;
    statistics.Ratings.forEach(function (rating) {
        if(rating.Map == map && rating.GameMode == gameMode)
            output = rating;
    });
    return output;
}

function renderGraph(summoner, statistics, games, additionalArguments)
{
    var map = getInteger(additionalArguments[0]);
    var gameMode = getInteger(additionalArguments[1]);
    var rating = getRating(statistics, map, gameMode);
    if(rating === null)
        throw 'Unable to locate rating';
    var wins = rating.Wins;
    var losses = rating.Losses;
    var gameIndex = wins + losses;
    var winLossDifference = wins - losses;
    var samples = [];
    var filteredGames = [];
    games.forEach(function(game) {
        if(game.Map == map && game.GameMode == gameMode)
        {
            var x = gameIndex;
            var y = winLossDifference;
            var sample = [x, y];
            samples.push(sample);
            game.winLossDifference = winLossDifference;
            filteredGames[gameIndex] = game;
            gameIndex--;
            winLossDifference -= game.Won ? 1 : -1;
        }
    });
    samples.reverse();

    var title = 'Win/Loss Graph for ' + summoner.SummonerName;
    setTitle(title);

    var header = header1(title);
    header.className = 'graphTitle';
    var filterList = list();
    filterList.className = 'filterDescription';
    var descriptions = [
        ['Map', getMapString(map)],
        ['Game mode', getGameModeString(gameMode)],
    ];
    descriptions.forEach(function (description) {
        var key = description[0];
        var value = description[1];
        var element = listElement();
        element.add(
            bold(key + ': '),
            value
        );
        filterList.add(element);
    });
    var outerContainer = diverse();
    outerContainer.className = 'graphContainer';
    var graphContainer = diverse();
    graphContainer.className = 'graph';
    var labelContainer = diverse();
    labelContainer.className = 'graphLabel';
    outerContainer.add(
        graphContainer,
        labelContainer
    );
    renderWithoutTemplate(
        header,
        filterList,
        outerContainer
    );

    var hiddenContainer = diverse();

    var highlightCallback = function(event, x, points, row)
    {
        var point = points[0];
        var gameIndex = point.xval;
        var game = filteredGames[gameIndex];
        var outcome = span(game.Won ? 'Win' : 'Loss');
        outcome.className = game.Won ? 'positive' : 'negative';
        labelContainer.purge();
        labelContainer.add([
            '[', getTimestampString(game.GameTime) + '] ',
            bold(outcome),
            ' with ' + getChampionName(game.ChampionId) + ' (',
            signum(game.winLossDifference),
            ')'
        ]);
    }

    var unhighlightCallback = function(event)
    {
        labelContainer.purge();
    }

    var options = {
        labels: ['Game', 'Win/loss difference'],
        drawPoints: true,
        colors: ['red'],
        labelsDiv: hiddenContainer,
        highlightCallback: highlightCallback,
        unhighlightCallback: unhighlightCallback,
    };
    new Dygraph(graphContainer, samples, options);
}