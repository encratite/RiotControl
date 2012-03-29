//Utility functions

function trimString()
{
    return this.replace(/^\s+|\s+$/g, '');
}

function apiCall(name, callArguments, callback)
{
    var path = '/API/' + name;
    for(var i in callArguments)
    {
        var argument = callArguments[i];
        path += '/' + argument;
    }
    var request = new XMLHttpRequest();
    request.onreadystatechange = function()
    {
        if(request.readyState == 4)
        {
            if(request.status == 200)
            {
                var response = JSON.parse(request.responseText);
                callback(response);
            }
            else if(request.status == 0)
            {
                //This appears to happen when you reload very fast
                //It probably means that the request was cancelled
                //Do nothing?
            }
            else
                throw 'API error in ' + path + ': ' + request.responseText + ' (' + request.status + ')';
        }
    }
    request.open('GET', path, true);
    request.send();
}

function getById(id)
{
    return document.getElementById(id);
}

function getResultString(response)
{
    var result = response.Result;
    var messages = [];
    messages['Success'] = 'Success.';
    messages['NotFound'] = 'Unable to find summoner.';
    messages['Timeout'] = 'The request has timed out.';
    messages['NotConnected'] = 'The worker for this region is not connected.';
    if(result in messages)
        return messages[result];
    else
        return 'Unknown server response (' + result + ').';
}

function isSuccess(response)
{
    return response.Result == 'Success';
}

function padWithZeroes(number, zeroes)
{
    if(zeroes === undefined)
        zeroes = 2;
    var output = number.toString();
    while(output.length < zeroes)
        output = '0' + output;
    return output;
}

function getTimestampString(timestamp)
{
    var date = new Date(timestamp * 1000);
    return date.getUTCFullYear() + '-' + padWithZeroes(date.getUTCMonth() + 1) + '-' + padWithZeroes(date.getUTCDate()) + ' ' + padWithZeroes(date.getUTCHours()) + ':' + padWithZeroes(date.getUTCMinutes()) + ':' + padWithZeroes(date.getUTCSeconds());
}

function getHashRequest()
{
    var hash = location.hash;
    if(hash.length <= 1)
    {
        //Default handler
        return null;
    }

    var tokens = hash.split('/');
    var name = tokens[0];
    if(name.length > 0 && name[0] == '#')
        name = name.substring(1);
    var requestArguments = tokens.slice(1);

    return new HashRequest(name, requestArguments);
}

function notImplemented()
{
    alert('This feature has not been implemented yet.');
}

function getSummonerRequest(requestArguments)
{
    if(requestArguments.length != 2)
        throw 'Invalid argument count.';
    var region = requestArguments[0];
    var accountId = requestArguments[1];
    var pattern = /^[1-9]\d*$/;
    if(pattern.exec(accountId) == null)
        throw 'Invalid account ID specified.';
    accountId = parseInt(accountId);
    for(i in system.regions)
    {
        var currentRegion = system.regions[i];
        if(currentRegion.abbreviation == region)
            return new SummonerRequest(region, accountId);
    }
    throw 'Invalid region specified.';
}

function signum(input)
{
    var node = span();
    if(input > 0)
    {
        node.className = 'positive';
        node.add('+' + input);
    }
    else if(input == 0)
        node.add('±0');
    else
    {
        input = - input;
        node.className = 'negative';
        node.add('-' + input);
    }
    return node;
}

function percentage(input)
{
    return (input * 100).toFixed(1) + '%';
}

function precisionString(input)
{
    if(input == Infinity)
        return '∞';
    else
        return input.toFixed(1);
}

function notAvailable()
{
    return '-';
}

function getCurrentRating(rating)
{
    if(rating.CurrentRating === null)
        return notAvailable();
    else
        return rating.CurrentRating;
}

function getTopRating(rating)
{
    if(rating.CurrentRating === null || rating.TopRating === null)
        return notAvailable();
    else
    {
        var node = span();
        node.add(rating.TopRating + ' (');
        node.add(signum(rating.CurrentRating - rating.TopRating));
        node.add(')');
        return node;
    }
}

function getSortingFunctionData(functionIndex)
{
    //The first entry contains the function that returns the value from the champion statistics that is going to be examined
    //The second entry defines the default isDescending value, i.e. default order
    var sortingFunctions =
        [
            [function (x) { return x.name; }, false],
            [function (x) { return x.gamesPlayed; }, true],
            [function (x) { return x.wins; }, true],
            [function (x) { return x.losses; }, true],
            [function (x) { return x.winLossDifference; }, true],
            [function (x) { return x.winRatio; }, true],
            [function (x) { return x.killsPerGame; }, true],
            [function (x) { return x.deathsPerGame; }, true],
            [function (x) { return x.assistsPerGame; }, true],
            [function (x) { return x.killsAndAssistsPerDeath; }, true],
            [function (x) { return x.minionKillsPerGame; }, true],
            [function (x) { return x.goldPerGame; }, true],
        ];
    return sortingFunctions[functionIndex];
}

function sortStatistics(statistics, functionIndex, isDescending)
{
    if(functionIndex === undefined)
        functionIndex = 0;
    if(isDescending === undefined)
        isDescending = false;
    var sortingFunctionData = getSortingFunctionData(functionIndex);
    var sortingFunction = sortingFunctionData[0];
    statistics.sort
    (
        function (a, b)
        {
            var x = sortingFunction(a);
            var y = sortingFunction(b);
            var sign = 1;
            if(isDescending)
                sign = -1;
            if(x > y)
                return sign;
            else if(x == y)
                return 0;
            else
                return - sign;
        }
    );
}

function initialiseSortableContainer(container)
{
    container.lastFunctionIndex = 0;
    container.isDescending = false;
}

function getSortableColumnFunction(description, statistics, i, containerName)
{
    return function()
    {
        sortStatisticsAndRender(description, statistics, i, containerName);
    }
}

function convertStatistics(statistics)
{
    var output = [];
    for(var i in statistics)
        output.push(new BasicStatistics(statistics[i]));
    return output;
}

//URL functions

function getURL(path)
{
    return system.baseURL + path;
}

function getBaseURL()
{
    var mainScript = getById('mainScript');
    if(mainScript === undefined)
        throw 'Unable to find the main script ID';
    var separator = '/';
    var tokens = mainScript.src.split(separator);
    if(tokens.length < 2)
        throw 'Invalid script path pattern';
    var baseURL = tokens.slice(0, -2).join(separator) + separator;
    return baseURL;
}

//Region class, used to hold information about regions in system.regions

function Region(abbreviation, description)
{
    this.abbreviation = abbreviation;
    this.description = description;
}

//Hash request class, used to store information about a # request string from the URL after it has been parsed

function HashRequest(name, requestArguments)
{
    this.name = name;
    this.requestArguments = requestArguments;
}

//Hash handler class, used to route hash requests to functions

function HashHandler(name, execute)
{
    this.name = name;
    this.execute = execute;
}

HashHandler.prototype.getHash = function()
{
    var requestArguments = [this.name];
    for(var i in arguments)
        requestArguments.push(arguments[i]);
    var path = '#' + requestArguments.join('/');
    return path;
};

//Classes to hold champion statistics for sorting the tables

function BasicStatistics(statistics)
{
    this.name = getChampionName(statistics.ChampionId);

    this.wins = statistics.Wins;
    this.losses = statistics.Losses;

    this.kills = statistics.Kills;
    this.deaths = statistics.Deaths;
    this.assists = statistics.Assists;

    this.minionKills = statistics.MinionKills;

    this.gold = statistics.Gold;

    var gamesPlayed = this.wins + this.losses;

    this.gamesPlayed = gamesPlayed;
    this.winLossDifference = this.wins - this.losses;

    this.killsPerGame = this.kills / gamesPlayed;
    this.deathsPerGame = this.deaths / gamesPlayed;
    this.assistsPerGame = this.assists / gamesPlayed;

    this.winRatio = this.wins / gamesPlayed;

    if(this.deaths > 0)
        this.killsAndAssistsPerDeath = (this.kills + this.assists) / this.deaths;
    else
        this.killsAndAssistsPerDeath = Infinity;

    this.minionKillsPerGame = this.minionKills / gamesPlayed;
    this.goldPerGame = this.gold / gamesPlayed;
}

function RankedStatistics(statistics)
{
    //Call base constructor
    this.base = BasicStatistics;
    this.base(statistics);

    this.turretsDestroyed = statistics.TurretsDestroyed;

    this.damageDealt = statistics.DamageDealt;
    this.physicalDamageDealt = statistics.PhysicalDamageDealt;
    this.magicalDamageDealt = statistics.MagicalDamageDealt

    this.damageTaken = statistics.DamageTaken;

    this.doubleKills = statistics.DoubleKills;
    this.tripleKills = statistics.TripleKills;
    this.quadraKills = statistics.QuadraKills;
    this.pentaKills = statistics.PentaKills;

    this.timeSpentDead = statistics.TimeSpentDead;

    this.maximumKills = statistics.MaximumKills;
    this.maximumDeaths = statistics.MaximumDeaths;
}

//Summoner request, a common type of hash request

function SummonerRequest(region, accountId)
{
    this.region = region;
    this.accountId = accountId;
}

//Item information class, used by LeagueOfLegends.js

function Item(name, description)
{
    this.name = name;
    this.description = description;
}

//Global initialisation

function initialiseSystem(regions, privileged)
{
    system = {};
    system.baseURL = getBaseURL();
    system.privileged = privileged;
    system.regions = [];
    for(i in regions)
    {
        var info = regions[i];
        var abbreviation = info[0];
        var description = info[1];
        var region = new Region(abbreviation, description);
        system.regions.push(region);
    }

    system.hashDefaultHandler = hashDefault;
    system.summonerHandler = new HashHandler('Summoner', hashViewSummoner);
    system.matchHistoryHandler = new HashHandler('Games', hashMatchHistory);
    system.hashHandlers =
        [
            system.summonerHandler,
            system.matchHistoryHandler,
        ];
}

function initialise(regions, privileged)
{
    initialiseSystem(regions, privileged);
    installExtensions();
    loadStylesheet();
    hashRouting();
}

function hashRouting()
{
    var request = getHashRequest();
    if(request == null)
    {
        system.hashDefaultHandler();
        return;
    }
    for(var i in system.hashHandlers)
    {
        var handler = system.hashHandlers[i];
        if(handler.name == request.name)
        {
            handler.execute(request.requestArguments);
            return;
        }
    }
    showError('Unknown hash path specified.');
}

function installExtensions()
{
    String.prototype.trim = trimString;

    //Used by renderWithoutTemplate
    document.body.add = addChild;
    document.body.purge = removeChildren;
}

//Content generation

function createElement(tag, children)
{
    var element = document.createElement(tag);
    //Extensions
    element.add = addChild;
    element.purge = removeChildren;
    if(children !== undefined)
    {
        for(var i in children)
        {
            var child = children[i];
            element.add(child);
        }
    }
    return element;
}

function addChild(input)
{
    if(typeof input == 'string')
        input = text(input);
    else if(typeof input == 'number')
        input = text(input.toString());
    //This hack makes my eyes bleed
    else if(input.concat)
    {
        for(var i in input)
            this.add(input[i]);
        return;
    }
    try
    {
        this.appendChild(input);
    }
    catch(exception)
    {
        //Firebug only
        if(console !== undefined)
            console.trace();
        throw exception;
    }
}

function removeChildren()
{
    while(this.hasChildNodes())
        this.removeChild(this.lastChild);
}

function text(text)
{
    var element = document.createTextNode(text);
    return element;
}

function image(path, description)
{
    var image = createElement('img');
    image.src = getURL('Image/' + path);
    image.alt = description;
    return image;
}

function diverse()
{
    return createElement('div', arguments);
}

function paragraph()
{
    return createElement('p', arguments);
}

function link(relationship, type, reference)
{
    var node = createElement('link');
    node.rel = relationship;
    node.type = type;
    node.href = reference;
    return node;
}

function stylesheet(path)
{
    return link('stylesheet', 'text/css', getURL(path));
}

function textBox(id)
{
    var node = createElement('input');
    node.type = 'text';
    node.className = 'text';
    node.id = id;
    return node;
}

function submitButton(description, handler)
{
    var node = createElement('input');
    node.type = 'submit';
    node.className = 'submit';
    node.value = description;
    node.onclick = handler;
    return node;
}

function select(id)
{
    var node = createElement('select');
    node.id = id;
    return node;
}

function option(description, value)
{
    var node = createElement('option');
    node.value = value;
    node.add(description);
    return node;
}

function span()
{
    return createElement('span', arguments);
}

function bold()
{
    return createElement('b', arguments);
}

function anchor(description, handler)
{
    var node = createElement('a');
    node.onclick = handler;
    node.add(description);
    return node;
}

function caption(title)
{
    var node = createElement('caption');
    node.add(title);
    return node;
}

function table()
{
    return createElement('table', arguments);
}

function tableRow()
{
    return createElement('tr', arguments);
}

function tableCell()
{
    return createElement('td', arguments);
}

function tableHead()
{
    return createElement('th', arguments);
}

//Riot Control JSON API

function apiFindSummoner(region, summonerName, callback)
{
    apiCall('Search', [region, summonerName], callback);
}

function apiUpdateSummoner(region, accountId, callback)
{
    apiCall('Update', [region, accountId], callback);
}

function apiGetSummonerProfile(region, accountId, callback)
{
    apiCall('Profile', [region, accountId], callback);
}

function apiGetMatchHistory(region, accountId, callback)
{
    apiCall('Games', [region, accountId], callback);
}

function apiSetAutomaticUpdates(region, accountId, enable, callback)
{
    apiCall('SetAutomaticUpdates', [region, accountId, enable ? 1 : 0], callback);
}

//Hash request handlers

function hashDefault()
{
    showIndex();
}

function hashViewSummoner(requestArguments)
{
    try
    {
        var request = getSummonerRequest(requestArguments);
    }
    catch(exception)
    {
        showError(exception);
        return;
    }
    loadingScreen();
    viewSummonerProfile(request.region, request.accountId);
}

function hashMatchHistory(requestArguments)
{
    try
    {
        var request = getSummonerRequest(requestArguments);
    }
    catch(exception)
    {
        showError(exception);
        return;
    }
    loadingScreen();
    viewMatchHistory(request.region, request.accountId);
}

//Rendering/DOM functions

function render()
{
    var targets = [];
    for(var i in arguments)
        targets.push(arguments[i]);
    var newContent = getTemplate(targets)
    document.body.purge();
    document.body.add(newContent);
}

function renderWithoutTemplate()
{
    document.body.purge();
    for(var i in arguments)
        document.body.add(arguments[i]);
}

function loadingScreen()
{
    render(bold('Loading...'));
}

function loadStylesheet()
{
    var node = stylesheet('Style/Style.css');
    document.head.appendChild(node);
}

function getTemplate()
{
    var logo = image('Logo.jpg');
    logo.id = 'logo';

    var content = diverse();
    content.id = 'content';

    for(var i in arguments)
        content.add(arguments[i]);

    return [logo, content];
}

function getRegionSelection()
{
    var selectNode = select('region');
    for(var i in system.regions)
    {
        var region = system.regions[i];
        var optionNode = option(region.description, region.abbreviation);
        selectNode.add(optionNode);
    }
    return selectNode;
}

function showIndex(deescriptionNode)
{
    location.hash = '';

    var container = diverse();
    container.id = 'indexForm';

    var description = paragraph();
    description.id = 'searchDescription';
    if(deescriptionNode === undefined)
        deescriptionNode = 'Enter the name of the summoner you want to look up:';
    description.add(deescriptionNode);
    var summonerNameField = textBox('summonerName');
    summonerNameField.onkeydown = function(event)
    {
        if(event.keyCode == 13)
            performSearch();
    };
    var regionSelection = getRegionSelection();
    var button = submitButton('Search', performSearch);
    button.id = 'searchButton';

    container.add(description);
    container.add(summonerNameField);
    container.add(regionSelection);
    container.add(button);

    render(container);
}

function showError(message)
{
    showIndex(getErrorSpan(message));
}

function showResponseError(response)
{
    showError(getResultString(response));
}

function setSearchFormState(state)
{
    var textBox = getById('summonerName');
    textBox.disabled = !state;
    var button = getById('searchButton');
    button.disabled = !state;
}

function getErrorSpan(message)
{
    var node = span();
    node.className = 'error';
    node.add(message);
    return node;
}

function searchError(message)
{
    var node = getErrorSpan(message);
    setSearchDescription(node);
    setSearchFormState(true);
}

function setSearchDescription(node)
{
    var paragraph = getById('searchDescription');
    paragraph.purge();
    paragraph.add(node);
}

function viewSummonerProfile(region, accountId)
{
    location.hash = system.summonerHandler.getHash(region, accountId);
    apiGetSummonerProfile(region, accountId, function (response) { onSummonerProfileRetrieval(response, region); } );
}

function renderSummonerProfile(profile)
{
    var overview = getSummonerOverview(profile);
    var ratings = getRatingTable(profile);

    var rankedStatistics = [];
    for(var i in profile.RankedStatistics)
        rankedStatistics.push(new RankedStatistics(profile.RankedStatistics[i]));

    render
    (
        overview,
        ratings,
        getStatisticsContainer('Ranked Statistics', 'rankedStatistics', rankedStatistics),
        getStatisticsContainer('Unranked Twisted Treeline Statistics', 'twistedTreelineStatistics', convertStatistics(profile.TwistedTreelineStatistics)),
        getStatisticsContainer("Unranked Summoner's Rift Statistics", 'summonersRiftStatistics', convertStatistics(profile.SummonersRiftStatistics)),
        getStatisticsContainer('Unranked Dominion Statistics', 'dominionStatistics', convertStatistics(profile.DominionStatistics))
    );
}

function getStatisticsContainer(description, containerName, statistics)
{
    sortStatistics(statistics);
    var container = diverse(getStatisticsTable(description, statistics, containerName));
    container.id = containerName;
    initialiseSortableContainer(container);
    return container;
}

function getAutomaticUpdateDescription(container, region, summoner)
{
    var output =
        [
            (summoner.UpdateAutomatically ? 'Yes' : 'No') + ' (',
            anchor(summoner.UpdateAutomatically ? 'disable' : 'enable', function() { setAutomaticUpdates(container, region, summoner, !summoner.UpdateAutomatically); } ),
            ')',
        ];

    return output;
}

function getSummonerOverview(profile)
{
    var summoner = profile.Summoner;
    var ratings = profile.Ratings;

    var region = system.regions[summoner.Region].abbreviation;

    var profileIcon = image('Profile/profileIcon' + summoner.ProfileIcon + '.jpg', summoner.SummonerName + "'s profile icon");
    profileIcon.id = 'profileIcon';

    var gamesPlayed = 0;
    for(i in ratings)
    {
        var statistics = ratings[i];
        gamesPlayed += statistics.Wins;
        gamesPlayed += statistics.Losses;
    }

    var overviewFields1 =
        [
            ['Summoner name', summoner.SummonerName],
            ['Internal name', summoner.InternalName],
            ['Region', region],
            ['Summoner level', summoner.SummonerLevel],
            ['Non-custom games played', gamesPlayed],
            ['Account ID', summoner.AccountId],
            ['Summoner ID', summoner.SummonerId],
        ];

    var matchHistoryLink = anchor('View games', function() { viewMatchHistory(region, summoner.AccountId); } );

    var overviewFields2 =
        [
            ['Match history', matchHistoryLink],
        ];

    var updateDescription = 'Is updated automatically';
    if(system.privileged)
    {
        //Requesting updates requires writing permissions
        var manualUpdateContainer = span();
        manualUpdateContainer.add(anchor('Update now', function() { updateSummoner(manualUpdateContainer, region, summoner.AccountId); } ));
        overviewFields2.push(['Manual update', manualUpdateContainer]);
        var automaticUpdateContainer = span();
        automaticUpdateContainer.add(getAutomaticUpdateDescription(automaticUpdateContainer, region, summoner));
        overviewFields2.push([updateDescription, automaticUpdateContainer]);
    }
    else
        overviewFields2.push([updateDescription, summoner.UpdateAutomatically ? 'Yes' : 'No']);

    overviewFields2 = overviewFields2.concat
    (
        [
            ['First update', getTimestampString(summoner.TimeCreated)],
            ['Last update', getTimestampString(summoner.TimeUpdated)],
        ]
    );

    var container = diverse();
    container.id = 'summonerHeader';
    container.add(profileIcon);
    container.add(getOverviewTable(overviewFields1));
    container.add(getOverviewTable(overviewFields2));

    return container;
}

function getOverviewTable(fields)
{
    var output = table();
    output.className = 'summonerOverview';
    for(var i in fields)
    {
        var entry = fields[i];
        var description = entry[0];
        var value = entry[1];

        var row = tableRow();
        row.add(tableCell(bold(description)));
        row.add(tableCell(value));
        output.add(row);
    }

    return output;
}

function getTableHeadRow(fields)
{
    var output = tableRow();
    for(var i in fields)
    {
        var field = fields[i];
        var cell = tableHead();
        cell.add(field);
        output.add(cell);
    }
    return output;
}

function getRatingTable(profile)
{
    var ratings = profile.Ratings;

    var columnTitles =
        [
            'Map',
            'Mode',
            'Games',
            'W',
            'L',
            'W - L',
            'WR',
            'Left',
            'Rating',
            'Top rating',
        ];

    var output = table();
    output.id = 'ratingTable';

    output.add(caption('General Statistics'));

    output.add(getTableHeadRow(columnTitles));

    var rowCount = 0;
    for(var i in ratings)
    {
        var rating = ratings[i];
        var gamesPlayed = rating.Wins + rating.Losses;
        if (gamesPlayed == 0)
            continue;
        var fields =
            [
                getMapString(rating.Map),
                getGameModeString(rating.GameMode),
                gamesPlayed,
                rating.Wins,
                rating.Losses,
                signum(rating.Wins - rating.Losses),
                percentage(rating.Wins / (rating.Wins + rating.Losses)),
                rating.Leaves,
                getCurrentRating(rating),
                getTopRating(rating),
            ];
        var row = tableRow();
        for(var i in fields)
            row.add(tableCell(fields[i]));
        output.add(row);
        rowCount++;
    }
    if(rowCount > 0)
        return output;
    else
        return '';
}

function getStatisticsTable(description, statistics, containerName)
{
    if(statistics.length == 0)
        return '';

    var output = table();

    output.className = 'statistics';
    output.add(caption(description));

    var columns =
        [
            'Champion',
            'Games',
            'W',
            'L',
            'W - L',
            'WR',
            'K',
            'D',
            'A',
            '(K + A) / D',
            'MK',
            'Gold',
        ];

    var row = tableRow();
    for(var i in columns)
    {
        var column = columns[i];
        var link = anchor(column, getSortableColumnFunction(description, statistics, i, containerName));
        row.add(tableHead(link));
    }
    output.add(row);

    for(var i in statistics)
    {
        var champion = statistics[i];

        var imageNode = span();
        imageNode.add(image('Champion/Small/' + encodeURI(champion.name) + '.png', champion.name));
        imageNode.add(champion.name);

        var fields =
            [
                imageNode,
                champion.gamesPlayed,
                champion.wins,
                champion.losses,
                signum(champion.winLossDifference),
                percentage(champion.wins / champion.gamesPlayed),
                precisionString(champion.killsPerGame),
                precisionString(champion.deathsPerGame),
                precisionString(champion.assistsPerGame),
                precisionString(champion.killsAndAssistsPerDeath),
                precisionString(champion.minionKillsPerGame),
                precisionString(champion.goldPerGame),
            ];

        var row = tableRow();
        for(var i in fields)
            row.add(tableCell(fields[i]));
        output.add(row);
    }

    return output;
}

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

    var output = table();
    output.id = 'summonerGames';
    output.className = 'statistics';

    output.add(caption('Games of ' + summoner.SummonerName));

    var row = tableRow();
    for(var i in titles)
        row.add(tableHead(titles[i]));
    output.add(row);
    for(var i in games)
    {
        var game = games[i];
        var championName = getChampionName(game.ChampionId);
        var championDescription = [image('Champion/Small/' + championName + '.png', championName), championName];

        var items = [];
        for(var i in game.Items)
        {
            var itemId = game.Items[i];
            if(itemId == 0)
                items.push(image('Item/Small/Blank.png', 'Unused'));
            else
            {
                var item = getItem(itemId);
                items.push(image('Item/Small/' + (item.description == 'Unknown' ? 'Unknown' : itemId) + '.png', item.name));
            }
        }

        var noValue = '-';
        var fields =
            [
                championDescription,
                getMapString(game.Map),
                getGameModeString(game.GameMode),
                getTimestampString(game.GameTime),
                game.IsBlueTeam ? 'Blue' : 'Purple',
                game.Kills,
                game.Deaths,
                game.Assists,
                game.MinionKills,
                game.NeutralMinionsKilled !== null ? game.NeutralMinionsKilled : noValue,
                game.Gold,
            ];
        var row = tableRow();
        console.log(game);
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
    return output;
}

//Button/link handlers

function performSearch()
{
    setSearchFormState(false);

    var summonerName = getById('summonerName').value;
    var regionSelect = getById('region');
    var region = regionSelect.options[regionSelect.selectedIndex].value;
    setSearchDescription('Searching for "' + summonerName + '"...');
    apiFindSummoner(region, summonerName, function (response) { onSearchResult(response, region); } );
}

function viewMatchHistory(region, accountId)
{
    location.hash = system.matchHistoryHandler.getHash(region, accountId);
    apiGetSummonerProfile(region, accountId, function (response) { onGetSummonerProfileForMatchHistory(response, region); } );
}

function updateSummoner(container, region, accountId)
{
    container.purge();
    container.add('Updating...');
    apiUpdateSummoner(region, accountId, function (response) { onSummonerUpdate(response, region, accountId); } );
}

function setAutomaticUpdates(container, region, summoner, enable)
{
    container.purge();
    container.add('Modifying settings...');
    apiSetAutomaticUpdates(region, summoner.AccountId, enable, function (response) { onSetAutomaticUpdates(response, container, region, summoner, enable); } );
}

function sortStatisticsAndRender(description, statistics, functionIndex, containerName)
{
    var container = document.getElementById(containerName);
    var isDescending;
    if (functionIndex == container.lastFunctionIndex)
        isDescending = !container.isDescending;
    else
    {
        var sortingFunctionData = getSortingFunctionData(functionIndex)
        isDescending = sortingFunctionData[1];
    }
    sortStatistics(statistics, functionIndex, isDescending);
    container.isDescending = isDescending;
    container.lastFunctionIndex = functionIndex;
    var statisticsTable = getStatisticsTable(description, statistics, containerName);
    container.purge();
    container.add(statisticsTable);
}

//API request handlers

function onSearchResult(response, region)
{
    if(isSuccess(response))
        viewSummonerProfile(region, response.AccountId);
    else
        searchError(getResultString(response));
}

function onSummonerProfileRetrieval(response, region)
{
    if(isSuccess(response))
    {
        var profile = response.Profile;
        var summoner = profile.Summoner;
        if(!summoner.HasBeenUpdated)
        {
            //This means that this summoner entry in the database was only created by a search for a summoner name.
            //It does not actually hold any useful information yet and needs to be updated first.
            apiUpdateSummoner(region, summoner.AccountId, function (response) { onSummonerUpdate(response, region, summoner.AccountId); } );
            return;
        }
        renderSummonerProfile(profile);
    }
    else
        showResponseError(response);
}

function onSummonerUpdate(response, region, accountId)
{
    if(isSuccess(response))
        viewSummonerProfile(region, accountId);
    else
        showResponseError(response);
}

function onSetAutomaticUpdates(response, container, region, summoner, enable)
{
    if(isSuccess(response))
    {
        summoner.UpdateAutomatically = enable;
        container.purge();
        container.add(getAutomaticUpdateDescription(container, region, summoner));
    }
    else
        showResponseError(response);
}

function onGetSummonerProfileForMatchHistory(response, region)
{
    if(isSuccess(response))
    {
        var profile = response.Profile;
        var summoner = profile.Summoner;
        apiGetMatchHistory(region, summoner.AccountId, function (response) { onGetMatchHistory(response, summoner); });
    }
    else
        showResponseError(response);
}

function onGetMatchHistory(response, summoner)
{
    if(isSuccess(response))
    {
        var table = getSummonerGamesTable(summoner, response.Games);
        renderWithoutTemplate(table);
    }
    else
        showResponseError(response);
}

//Translating enumerated types

function getMapString(map)
{
    switch(map)
    {
    case 0:
        return 'Twisted Treeline';
    case 1:
        return "Summoner's Rift";
    case 2:
        return 'Dominion';
    default:
        return 'Unknown';
    }
}

function getGameModeString(mode)
{
    switch(mode)
    {
    case 0:
        return 'Custom';
    case 1:
        return 'Co-op vs. AI';
    case 2:
        return 'Normal';
    case 3:
        return 'Ranked Solo/Duo';
    case 4:
        return 'Ranked Teams';
    default:
        return 'Unknown';
    }
}