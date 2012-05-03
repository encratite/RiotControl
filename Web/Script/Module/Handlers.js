function installHandlers()
{
    system.defaultHandler = defaultHandler;
    system.summonerHandler = new RequestHandler('Summoner', viewSummonerHandler);
    system.matchHistoryHandler = new RequestHandler('Games', matchHistoryHandler);
    system.runesHandler = new RequestHandler('Runes', viewRunesHandler);
    system.graphHandler = new RequestHandler('Graph', graphHandler);
    system.requestHandlers =
        [
            system.summonerHandler,
            system.matchHistoryHandler,
            system.runesHandler,
            system.graphHandler,
        ];
}

function defaultHandler()
{
    if(hasSearchPrivilege())
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

function graphHandler(requestArguments)
{
    processSummonerRequest(requestArguments, viewGraph);
}