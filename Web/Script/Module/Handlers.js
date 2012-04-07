function installHandlers()
{
    system.defaultHandler = defaultHandler;
    system.summonerHandler = new RequestHandler('Summoner', viewSummonerHandler);
    system.matchHistoryHandler = new RequestHandler('Games', matchHistoryHandler);
    system.runesHandler = new RequestHandler('Runes', viewRunesHandler);
    system.requestHandlers =
        [
            system.summonerHandler,
            system.matchHistoryHandler,
            system.runesHandler,
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
    processSummonerRequest(requestArguments, viewSummonerProfile);
}

function matchHistoryHandler(requestArguments)
{
    processSummonerRequest(requestArguments, viewMatchHistory);
}

function viewRunesHandler(requestArguments)
{
    processSummonerRequest(requestArguments, viewRunes);
}