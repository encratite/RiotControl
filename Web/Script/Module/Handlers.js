function installHandlers()
{
    system.defaultHandler = defaultHandler;
    system.summonerHandler = new RequestHandler('Summoner', viewSummonerHandler);
    system.matchHistoryHandler = new RequestHandler('Games', matchHistoryHandler);
    system.requestHandlers =
        [
            system.summonerHandler,
            system.matchHistoryHandler,
        ];
}

function defaultHandler()
{
    if(system.privileged)
        showIndex();
    else
        showError('You do not have permission to issue searches.');
}

function viewSummonerHandler(requestArguments)
{
    try
    {
        var request = getSummonerRequest(requestArguments);
    }
    catch(exception)
    {
        showError(exception.message);
        return;
    }
    viewSummonerProfile(request.region, request.accountId);
}

function matchHistoryHandler(requestArguments)
{
    try
    {
        var request = getSummonerRequest(requestArguments);
    }
    catch(exception)
    {
        showError(exception.message);
        return;
    }
    viewMatchHistory(request.region, request.accountId);
}