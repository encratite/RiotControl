//Utility functions

function setTitle(title)
{
    document.title = title + ' - Riot Control r' + system.revision;
}

function apiCall(name, callArguments, callback)
{
    var path = '/API/' + name;
    callArguments.forEach(function(argument) {
        path += '/' + argument;
    });
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

function getRequest()
{
    var separator = '/';
    var url = location.href;
    var offset = url.indexOf(separator, 8);
    if(offset == -1)
        throw 'Unable to parse request path';
    var path = url.substring(offset + 1);
    if(path.length == 0)
        return null;
    //Remove hash component
    offset = path.indexOf('#');
    if(offset != -1)
        path = path.substring(0, offset);
    var tokens = path.split(separator);
    if(tokens.length == 0)
        return null;
    var name = tokens[0];
    var requestArguments = tokens.slice(1);

    return new Request(name, requestArguments);
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
    if(pattern.exec(accountId) === null)
        throw 'Invalid account ID specified.';
    accountId = parseInt(accountId);
    var regions = system.regions;
    for(var i in regions)
    {
        var currentRegion = regions[i];
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
            if(a.isSummary)
                return 1;
            else if(b.isSummary)
                return -1;
            var x = sortingFunction(a);
            var y = sortingFunction(b);
            var sign = 1;
            if(isDescending)
                sign = -1;
            if(x > y)
                return sign;
            else if(x === y)
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

function parseArguments(argumentObject)
{
    return Array.prototype.slice.call(argumentObject);
}

function processCookieString(string)
{
    return unescape(string.trim());
}

function getCookies()
{
    var output = {};
    var input = document.cookie;
    if(input.length > 0)
    {
        var tokens = input.split(';');
        tokens.forEach(function(token) {
            var innerTokens = token.split('=');
            if(innerTokens.length != 2)
                throw 'Invalid cookie format';
            var name = processCookieString(innerTokens[0]);
            var value = processCookieString(innerTokens[1]);
            output[name] = value;
        });
    }
    return output;
}

function getCookie(name)
{
    var cookies = getCookies();
    var value = cookies[name];
    if(value !== undefined)
        return value;
    else
        return null;
}

function setCookie(name, value)
{
    document.cookie = name + '=' + value + '; Path=/';
}

function setSearchRegion(region)
{
    setCookie('searchRegion', region);
}

function getSearchRegion()
{
    var abbreviation = getCookie('searchRegion');
    if(abbreviation === null)
        return null;
    var regions = system.regions;
    for(var i in regions)
    {
        var region = regions[i];
        if(region.abbreviation == abbreviation)
            return region;
    }
    return null;
}

function processSummonerRequest(requestArguments, handler)
{
    try
    {
        var request = getSummonerRequest(requestArguments);
    }
    catch(exception)
    {
        if(exception.message)
            exception = exception.message;
        showError(exception);
        return;
    }
    handler(request.region, request.accountId);
}

function trace()
{
    if(console !== undefined)
        console.trace();
}

function log(object)
{
    if(console !== undefined)
        console.log(object);
}

function getRegion(identifier)
{
    var region = system.regions[identifier];
    if(region === undefined)
        return null;
    else
        return region;
}