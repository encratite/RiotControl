function getSummonerGamesTable(summoner, games)
{
    var titles =
        [
            'Champion',
            'Map',
            'Mode',
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
    for(var i in titles)
        row.add(tableHead(titles[i]));
    output.add(row);
    for(var i in games)
    {
        var game = games[i];
        var championName = getChampionName(game.ChampionId);
        var championDescription = [icon('Champion/Small/' + championName + '.png', championName), championName];

        var items = [];
        for(var i in game.Items)
        {
            var itemId = game.Items[i];
            if(itemId == 0)
                items.push(icon('Item/Small/Blank.png', 'Unused'));
            else
            {
                var item = getItem(itemId);
                items.push(icon('Item/Small/' + (item.description == 'Unknown' ? 'Unknown' : itemId) + '.png', item.name));
            }
        }

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
        for(var i in fields)
            row.add(tableCell(fields[i]));
        var itemsCell = tableCell(items);
        itemsCell.className = 'items';
        row.add(itemsCell);
        var premadeString;
        //Check if it's a custom game
        if(game.GameMode == 0)
            premadeString = noValue;
        else
            premadeString = game.PremadeSize <= 1 ? 'No' : 'Yes, ' + game.PremadeSize;
        var queueTimeString = game.TimeSpentInQueue > 0 ? game.TimeSpentInQueue + ' s' : noValue;
        fields =
            [
                premadeString,
                game.Ping + ' ms',
                queueTimeString,
                game.InternalGameId,
            ];
        for(var i in fields)
            row.add(tableCell(fields[i]));
        output.add(row);
    }
    return container;
}

function renderMatchHistory(region, summoner, games)
{
    setTitle('Games of ' + summoner.SummonerName);
    var linkContainer = paragraph(anchor('Return to profile', function () { viewSummonerProfile(region, summoner.AccountId); } ));
    linkContainer.id = 'returnToProfile';
    var table = getSummonerGamesTable(summoner, games);
    renderWithoutTemplate(linkContainer, table);
}