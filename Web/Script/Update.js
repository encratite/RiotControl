function processStateChange(request)
{
    if(request.readyState == 1)
        setStatus('Updating...');
    else if(request.readyState == 4)
    {
        var response = JSON.parse(request.responseText);
        var result = response['Result'];
        var summonerLevel = response['SummonerLevel'];
        var output;
        switch(result)
        {
        case 'Success':
            window.location.reload();
            return;
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
        setStatus(output);
    }
}

function setStatus(message)
{
    document.getElementById('manualUpdate').innerHTML = message;
}

function updateSummoner(region, accountId)
{
    var request = new XMLHttpRequest();
    request.onreadystatechange = function()
    {
        processStateChange(request);
    }
    var path = '/RiotControl/UpdateSummoner/' + region + '/' + accountId;
    request.open('GET', path, true);
    request.send();
}