function hashDefault()
{
    if(system.privileged)
        showIndex();
    else
        showError('You do not have permission to issue searches.');
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