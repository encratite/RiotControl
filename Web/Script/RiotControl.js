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
            else
                throw 'API error in ' + path + ': ' + request.responseText;
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
    return date.getFullYear() + '-' + padWithZeroes(date.getMonth()) + '-' + padWithZeroes(date.getDay()) + ' ' + padWithZeroes(date.getHours()) + ':' + padWithZeroes(date.getMinutes()) + ':' + padWithZeroes(date.getSeconds());
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

//Global initialisation

function initialiseSystem(regions, privileged)
{
    system = {};
    system.baseURL = getBaseURL();
    system.content = getTemplate()
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
    system.hashHandlers =
        [
            system.summonerHandler,
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
    apiCall('Search', [region, summonerName], function (response) { callback(region, response); } );
}

function apiUpdateSummoner(region, accountId, callback)
{
    apiCall('Update', [region, accountId], function (response) { callback(region, response); });
}

function apiGetSummonerProfile(region, accountId, callback)
{
    apiCall('Profile', [region, accountId], function (response) { callback(region, response); });
}

function apiGetMatchHistoryu(region, accountId, callback)
{
    apiCall('Games', [region, accountId], function (response) { callback(region, response); });
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
    viewSummonerProfile(request.region, request.accountId);
}

//Rendering/DOM functions

function render()
{
    system.content.purge();
    for(var i in arguments)
        system.content.add(arguments[i]);
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

    document.body.appendChild(logo);
    document.body.appendChild(content);

    return content;
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
    apiGetSummonerProfile(region, accountId, onSummonerProfileRetrieval);
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

function getAutomaticUpdateDescription(region, summoner)
{
    var output;
    if(summoner.UpdateAutomatically)
    {
        output =
            [
                'Yes (',
                anchor('disable', function() { setAutomaticUpdates(region, summoner, false); } ),
                ')',
            ];
    }
    else
    {
        output =
            [
                'No (',
                anchor('enable', function() { setAutomaticUpdates(region, summoner, true); } ),
                ')',
            ];
    }
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
        var manualUpdateLink = anchor('Update now', function() { updateSummoner(region, summoner.AccountId); } );
        overviewFields2.push(['Manual update', manualUpdateLink]);
        var automaticUpdateDescription = span();
        automaticUpdateDescription.id = 'automaticUpdates';
        automaticUpdateDescription.add(getAutomaticUpdateDescription(region, summoner));
        overviewFields2.push([updateDescription, automaticUpdateDescription]);
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

//Button/link handlers

function performSearch()
{
    setSearchFormState(false);

    var summonerName = getById('summonerName').value;
    var regionSelect = getById('region');
    var region = regionSelect.options[regionSelect.selectedIndex].value;
    setSearchDescription('Searching for "' + summonerName + '"...');
    apiFindSummoner(region, summonerName, onSearchResult);
}

function viewMatchHistory(region, accountId)
{
    notImplemented();
}

function updateSummoner(region, accountId)
{
    notImplemented();
}

function setAutomaticUpdates(region, accountId, enable)
{
    notImplemented();
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

function onSearchResult(region, response)
{
    if(isSuccess(response))
        viewSummonerProfile(region, response.AccountId);
    else
        searchError(getResultString(response));
}

function onSummonerProfileRetrieval(region, response)
{
    if(isSuccess(response))
    {
        var profile = response.Profile;
        var summoner = profile.Summoner;
        if(!summoner.HasBeenUpdated)
        {
            //This means that this summoner entry in the database was only created by a search for a summoner name.
            //It does not actually hold any useful information yet and needs to be updated first.
            apiUpdateSummoner(region, summoner.AccountId, function (region, response) { onSummonerUpdate(region, summoner.AccountId, response); } );
            return;
        }
        renderSummonerProfile(profile);
    }
    else
        showResponseError(response);
}

function onSummonerUpdate(region, accountId, response)
{
    if(isSuccess(response))
        viewSummonerProfile(region, accountId);
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

//Champion name translation

function getChampionName(championId)
{
    switch(championId)
    {
    case 1: return "Annie";
    case 2: return "Olaf";
    case 3: return "Galio";
    case 4: return "Twisted Fate";
    case 5: return "Xin Zhao";
    case 6: return "Urgot";
    case 7: return "LeBlanc";
    case 8: return "Vladimir";
    case 9: return "Fiddlesticks";
    case 10: return "Kayle";
    case 11: return "Master Yi";
    case 12: return "Alistar";
    case 13: return "Ryze";
    case 14: return "Sion";
    case 15: return "Sivir";
    case 16: return "Soraka";
    case 17: return "Teemo";
    case 18: return "Tristana";
    case 19: return "Warwick";
    case 20: return "Nunu";
    case 21: return "Miss Fortune";
    case 22: return "Ashe";
    case 23: return "Tryndamere";
    case 24: return "Jax";
    case 25: return "Morgana";
    case 26: return "Zilean";
    case 27: return "Singed";
    case 28: return "Evelynn";
    case 29: return "Twitch";
    case 30: return "Karthus";
    case 31: return "Cho'Gath";
    case 32: return "Amumu";
    case 33: return "Rammus";
    case 34: return "Anivia";
    case 35: return "Shaco";
    case 36: return "Dr. Mundo";
    case 37: return "Sona";
    case 38: return "Kassadin";
    case 39: return "Irelia";
    case 40: return "Janna";
    case 41: return "Gangplank";
    case 42: return "Corki";
    case 43: return "Karma";
    case 44: return "Taric";
    case 45: return "Veigar";
    case 48: return "Trundle";
    case 50: return "Swain";
    case 51: return "Caitlyn";
    case 53: return "Blitzcrank";
    case 54: return "Malphite";
    case 55: return "Katarina";
    case 56: return "Nocturne";
    case 57: return "Maokai";
    case 58: return "Renekton";
    case 59: return "Jarvan IV";
    case 61: return "Orianna";
    case 62: return "Wukong";
    case 63: return "Brand";
    case 64: return "Lee Sin";
    case 67: return "Vayne";
    case 68: return "Rumble";
    case 69: return "Cassiopeia";
    case 72: return "Skarner";
    case 74: return "Heimerdinger";
    case 75: return "Nasus";
    case 76: return "Nidalee";
    case 77: return "Udyr";
    case 78: return "Poppy";
    case 79: return "Gragas";
    case 80: return "Pantheon";
    case 81: return "Ezreal";
    case 82: return "Mordekaiser";
    case 83: return "Yorick";
    case 84: return "Akali";
    case 85: return "Kennen";
    case 86: return "Garen";
    case 89: return "Leona";
    case 90: return "Malzahar";
    case 91: return "Talon";
    case 92: return "Riven";
    case 96: return "Kog'Maw";
    case 98: return "Shen";
    case 99: return "Lux";
    case 101: return "Xerath";
    case 102: return "Shyvana";
    case 103: return "Ahri";
    case 104: return "Graves";
    case 105: return "Fizz";
    case 106: return "Volibear";
    case 111: return "Nautilus";
    case 112: return "Viktor";
    case 113: return "Sejuani";
    case 114: return "Fiora";
    case 115: return "Ziggs";
    default: return 'Champion ' + championId;
    }
}