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

function integerTicker(min, max, pixels, options, dygraph, values)
{
    var output = Dygraph.numericTicks(min, max, pixels, options, dygraph, values);
    output = output.filter(function (entry) {
        return entry.v == Math.floor(entry.v);
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
    var gameCount = 0;
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
            gameCount++;
        }
    });
    samples.reverse();

    var title = 'Win/Loss Graph for ' + summoner.SummonerName;
    setTitle(title);

    var explanation = paragraph('The x-axis shows the number of games played in the particular game mode, while the y-axis shows the development of the difference between wins and losses over time.');
    explanation.id = 'graphExplanation';

    var description = table();
    description.className = 'graphDescription';
    var summonerLink = anchor(summoner.SummonerName, function () { system.summonerHandler.open(getRegion(summoner.Region).abbreviation, summoner.AccountId); } );
    var descriptionFields = [
        ['Summoner', summonerLink],
        ['Map', getMapString(map)],
        ['Game mode', getGameModeString(gameMode)],
        ['Games in database', gameCount],
    ];
    descriptionFields.forEach(function (field) {
        var key = field[0];
        var value = field[1];
        var row = tableRow();
        row.add(tableCell(bold(key)));
        row.add(tableCell(value));
        description.add(row);
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
    renderWithSearchForm(
        description,
        explanation,
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
            ' with ' + getChampionName(game.ChampionId) + ' (game #' + gameIndex + ', ',
            signum(game.winLossDifference),
            ')'
        ]);
    }

    var unhighlightCallback = function(event)
    {
        labelContainer.purge();
    }

    var options = {
        drawPoints: true,
        drawXGrid: false,
        ticker: integerTicker,
        labels: ['Game', 'Win/loss difference'],
        colors: ['red'],
        labelsDiv: hiddenContainer,
        highlightCallback: highlightCallback,
        unhighlightCallback: unhighlightCallback,
    };
    new Dygraph(graphContainer, samples, options);
}