function processStateChange(region, request)
{
    var output;
    if(request.readyState == 1)
    {
        request.timer = getMilliseconds();
        setResult(region, 'Waiting for a response from the server');
        setDuration(region, 'Unknown');
    }
    else if(request.readyState == 4)
    {
        var response = JSON.parse(request.responseText);
        var result = response['Result'];
        var summonerName = response['SummonerName'];
        var accountId = response['AccountID'];
        switch(result)
        {
        case 'Success':
            var path = '/RiotControl/Summoner/' + region + '/' + accountId;
            output = '<a href="' + path + '">' + summonerName + '</a> (' + accountId + ')';
            break;
        case 'NotFound':
            output = 'No such summoner';
            break;
        case 'Timeout':
            output = 'Request timed out';
            break;
        default:
            output = 'Unknown response';
            break;
        }
        var duration = getMilliseconds() - request.timer;
        setResult(region, output);
        setDuration(region, duration + ' ms');
    }
}

function getMilliseconds()
{
    var now = new Date();
    return Math.round(now.getTime());
}

function setCell(prefix, region, content)
{
    var id = prefix + region;
    document.getElementById(id).innerHTML = content;
}

function setResult(region, result)
{
    setCell('Result', region, result);
}

function setDuration(region, duration)
{
    setCell('Duration', region, duration);
}

function findSummonerInRegion(region, summonerName)
{
    var request = new XMLHttpRequest();
    request.onreadystatechange = function()
    {
        processStateChange(region, request);
    }
    var path = '/RiotControl/FindSummoner/' + region + '/' + encodeURI(summonerName);
    request.open('GET', path, true);
    request.send();
}

function findSummoner(summonerName)
{
    var regions =
        [
            'NA',
            'EUW',
            'EUNE',
        ];
    for(var i in regions)
        findSummonerInRegion(regions[i], summonerName);
}