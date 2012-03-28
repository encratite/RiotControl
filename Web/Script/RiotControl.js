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

//Region class

function Region(abbreviation, description)
{
    this.abbreviation = abbreviation;
    this.description = description;
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
}

function initialise(regions, privileged)
{
    initialiseSystem(regions, privileged);
    installStringExtensions();
    loadStylesheet();
    showIndex();
}

function installStringExtensions()
{
    String.prototype.trim = trimString;
}

//Content generation

function createElement(tag)
{
    var element = document.createElement(tag);
    //Extensions
    element.add = addChild;
    element.purge = removeChildren;
    return element;
}

function addChild(input)
{
    if(typeof input == 'string')
        input = text(input);
    else if(typeof input == 'number')
        input = text(input.toString());
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
    return createElement('div');
}

function paragraph()
{
    return createElement('p');
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
    return createElement('span');
}

function bold()
{
    return createElement('b');
}

function anchor()
{
    return createElement('a');
}

function table()
{
    return createElement('table');
}

function tableRow()
{
    return createElement('tr');
}

function tableCell()
{
    return createElement('td');
}

function tableHead()
{
    return createElement('th');
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

//Rendering/DOM functions

function render(node)
{
    system.content.purge();
    system.content.add(node);
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

function showIndex(errorResponse)
{
    var container = diverse();
    container.id = 'indexForm';

    var description = paragraph();
    description.id = 'searchDescription';
    if(errorResponse === undefined)
        description.add('Enter the name of the summoner you want to look up:');
    else
        description.add(getErrorSpan(errorResponse.Result));
    var summonerNameField = textBox('summonerName');
    var regionSelection = getRegionSelection();
    var button = submitButton('Search', performSearch);
    button.id = 'searchButton';

    container.add(description);
    container.add(summonerNameField);
    container.add(regionSelection);
    container.add(button);

    render(container);
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
    apiGetSummonerProfile(region, accountId, onSummonerProfileRetrieval);
}

function renderSummonerProfile(profile)
{
    var summoner = profile.Summoner;
    var ratings = profile.Ratings;
    var rankedStatistics = profile.RankedStatistics;
    var unrankedStatistics = profile.UnrankedStatistics;
    var dominionStatistics = profile.DominionStatistics;

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

    var updateNowLink = anchor();
    updateNowLink.onclick = function()
    {
        updateSummoner(region, summoner.AccountId);
    };
    updateNowLink.add('Update now');

    var automaticUpdatesLink = anchor();
    automaticUpdatesLink.onclick = function()
    {
        enableAutomaticUpdates(region, summoner.AccountId);
    };
    automaticUpdatesLink.add('enable');

    var automaticUpdatesDescription = span();
    automaticUpdatesDescription.id = 'automaticUpdates';
    automaticUpdatesDescription.add('No (');
    automaticUpdatesDescription.add(automaticUpdatesLink);
    automaticUpdatesDescription.add(')');

    var matchHistory = anchor();
    matchHistory.onclick = function()
    {
        viewMatchHistory(region, summoner.AccountId);
    };
    matchHistory.add('View games');

    var overviewFields2 =
    [
        ['Match history', matchHistory],
        ['Manual update', updateNowLink],
        ['Is updated automatically', summoner.UpdateAutomatically ? 'Yes' : automaticUpdatesDescription],
        ['First update', getTimestampString(summoner.TimeCreated)],
        ['Last update', getTimestampString(summoner.TimeUpdated)],
    ];

    var container = diverse();
    container.id = 'summonerHeader';
    container.add(profileIcon);
    container.add(getOverviewTable(overviewFields1));
    container.add(getOverviewTable(overviewFields2));

    render(container);
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

        var descriptionCell = tableCell();
        var boldDescription = bold();
        boldDescription.add(description);
        descriptionCell.add(boldDescription);

        var valueCell = tableCell();
        valueCell.add(value);

        row.add(descriptionCell);
        row.add(valueCell);

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
}

function updateSummoner(region, accountId)
{
}

function enableAutomaticUpdates(region, accountId)
{
}

//JSON request handlers

function onSearchResult(region, response)
{
    if(isSuccess(response))
        viewSummonerProfile(region, response.AccountId);
    else
        searchError(response);
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
        showIndex(response);
}

function onSummonerUpdate(region, accountId, response)
{
    if(isSuccess(response))
        renderSummonerProfile(response.Profile);
    else
        showIndex(response);
}