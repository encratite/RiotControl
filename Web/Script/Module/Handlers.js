//Install system handlers

installHandlers();

//Hash request handlers

function installHandlers()
{
    system.hashDefaultHandler = hashDefault;
    system.summonerHandler = new HashHandler('Summoner', hashViewSummoner);
    system.matchHistoryHandler = new HashHandler('Games', hashMatchHistory);
    system.hashHandlers =
        [
            system.summonerHandler,
            system.matchHistoryHandler,
        ];
}

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