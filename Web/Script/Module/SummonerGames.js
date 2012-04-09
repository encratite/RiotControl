function getSummonerGamesTable(summoner, games)
{
    var titles =
        [
            'Champion',
            'Map',
            'Mode',
            'Outcome',
            'Date',
            'Side',
            'K',
            'D',
            'A',
            'MK',
            'NK',
            'Gold',
            'Items',
            'Premade',
            'Ping',
            'Time in queue',
            'Game ID',
        ];

    var container = diverse();
    container.id = 'summonerGamesContainer';

    var output = table();
    output.id = 'summonerGames';
    output.className = 'statistics';

    container.add(output);

    output.add(caption('Games of ' + summoner.SummonerName));

    var row = tableRow();
    titles.forEach(function(title) {
        row.add(tableHead(title));
    });
    output.add(row);
    games.forEach(function(game) {
        var championName = getChampionName(game.ChampionId);
        var championDescription = [icon('Champion/Small/' + championName + '.png', championName), championName];

        var items = [];
        game.Items.forEach(function(itemId) {
            if(itemId == 0)
                items.push(icon('Item/Small/Blank.png', 'Unused'));
            else
            {
                var item = getItem(itemId);
                items.push(icon('Item/Small/' + (item.description == 'Unknown' ? 'Unknown' : itemId) + '.png', item.name));
            }
        });

        var blue = span('Blue');
        blue.className = 'blue';

        var purple = span('Purple');
        purple.className = 'purple';

        var noValue = '-';
        var fields =
            [
                championDescription,
                getMapString(game.Map),
                getGameModeString(game.GameMode),
                game.Won ? 'Win' : 'Loss',
                getTimestampString(game.GameTime),
                game.IsBlueTeam ? blue : purple,
                game.Kills,
                game.Deaths,
                game.Assists,
                game.MinionKills,
                game.NeutralMinionsKilled !== null ? game.NeutralMinionsKilled : noValue,
                game.Gold,
            ];
        var row = tableRow();
        row.className = game.Won ? 'win' : 'loss';
        fields.forEach(function(field) {
            row.add(tableCell(field));
        });
        var itemsCell = tableCell(items);
        itemsCell.className = 'items';
        row.add(itemsCell);
        var premadeString;
        if(game.GameMode == 4)
        {
            //Override this value for ranked teams because the data provided by the server is invalid
            game.PremadeSize = game.Map == 0 ? 3 : 5;
        }
        //Check if it's a custom game
        if(game.GameMode == 0)
            premadeString = noValue;
        else
            premadeString = game.PremadeSize > 1 ? 'Yes, ' + game.PremadeSize : 'No';
        var queueTimeString = game.TimeSpentInQueue > 0 ? game.TimeSpentInQueue + ' s' : noValue;
        fields =
            [
                premadeString,
                game.Ping + ' ms',
                queueTimeString,
                game.InternalGameId,
            ];
        fields.forEach(function(field) {
            row.add(tableCell(field));
        });
        output.add(row);
    });
    return container;
}

function renderMatchHistory(summoner, games)
{
    setTitle('Games of ' + summoner.SummonerName);
    var linkContainer = paragraph(anchor('Return to profile', function () { system.summonerHandler.open(getRegion(summoner.Region).abbreviation, summoner.AccountId); } ));
    linkContainer.id = 'returnFromMatchHistory';
    var table = getSummonerGamesTable(summoner, games);
    renderWithoutTemplate(linkContainer, table);
}

function viewMatchHistory(region, accountId)
{
    apiGetSummonerProfile(region, accountId, function (response) { onGetSummonerProfileForMatchHistory(response, region); } );
}