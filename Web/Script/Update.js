function processManualUpdateStateChange(request)
{
    if(request.readyState == 1)
        setManualUpdateStatus('Updating...');
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
        setManualUpdateStatus(output);
    }
}

function processAutomaticUpdatesStateChange(request)
{
    if(request.readyState == 1)
        setAutomaticUpdatesStatus('Changing settings...');
    else if(request.readyState == 4)
    {
        var response = JSON.parse(request.responseText);
        var success = response['Success'];
        var output;
        if(success)
            output = 'Yes';
        else
            output = 'Failed to update account';
        setAutomaticUpdatesStatus(output);
    }
}

function setManualUpdateStatus(message)
{
    document.getElementById('manualUpdate').innerHTML = message;
}

function setAutomaticUpdatesStatus(message)
{
    document.getElementById('automaticUpdates').innerHTML = message;
}

function performAccountRequest(region, accountId, handlerName, stateChangeHandler)
{
    var request = new XMLHttpRequest();
    request.onreadystatechange = function()
    {
        stateChangeHandler(request);
    }
    var path = '/RiotControl/' + handlerName + '/' + region + '/' + accountId;
    request.open('GET', path, true);
    request.send();
}

function updateSummoner(region, accountId)
{
    performAccountRequest(region, accountId, 'LoadAccountData', processManualUpdateStateChange);
}

function enableAutomaticUpdates(region, accountId)
{
    performAccountRequest(region, accountId, 'AutomaticUpdates', processAutomaticUpdatesStateChange);
}