//API request handlers

function onSearchResult(response, region)
{
    if(isSuccess(response))
        system.summonerHandler.open(region, response.AccountId);
    else
        showResponseError(response);
}

function onGetSummonerProfile(response, region, accountId)
{
    var update = function() { apiUpdateSummoner(region, accountId, function (response) { onSummonerUpdate(response, region, accountId); } ); };
    if(isSuccess(response))
    {
        var summoner = response.Summoner;
        if(summoner.HasBeenUpdated)
        {
            //The summoner is ready to be displayed, we just need to load the actual statistics from the SQLite database first
            apiGetSummonerStatistics(region, accountId, function (response) { onGetSummonerStatistics(response, summoner); } );
        }
        else
        {
            //This means that this summoner entry in the database was only created by a search for a summoner name.
            //It does not actually hold any useful information yet and needs to be updated first.
            if(system.privileged)
            {
                //Only privileged users may request these updates
                update();
            }
            else
            {
                //Non-privileged users can't do anything about the absence of information so we'll just have to display an error message instead
                showError('Summoner has not been fully loaded yet.');
            }
        }
    }
    else
    {
        if(system.privileged && response.Result == 'NotFound')
        {
            //The summoner was not found in the database but they might still be available on the server
            //After all, this might have been a link to a summoner profile provided by somebody else
            //This is not an option available to non-privileged users, though, as they have no writing permissions and can't issue the required update
            update();
        }
        else
            showResponseError(response);
    }
}

function onGetSummonerStatistics(response, summoner)
{
    if(isSuccess(response))
        renderSummonerProfile(summoner, response.Statistics);
    else
        showResponseError(response);
}

function onSummonerUpdate(response, region, accountId)
{
    if(isSuccess(response))
        viewSummonerProfile(region, accountId);
    else
        showResponseError(response);
}

function onSetAutomaticUpdates(response, container, region, summoner, enable)
{
    if(isSuccess(response))
    {
        summoner.UpdateAutomatically = enable;
        container.purge();
        container.add(getAutomaticUpdateDescription(container, region, summoner));
    }
    else
        showResponseError(response);
}

function onGetSummonerProfileForMatchHistory(response, region)
{
    if(isSuccess(response))
    {
        var summoner = response.Summoner;
        apiGetMatchHistory(region, summoner.AccountId, function (response) { onGetMatchHistory(response, region, summoner); });
    }
    else
        showResponseError(response);
}

function onGetMatchHistory(response, region, summoner)
{
    if(isSuccess(response))
        renderMatchHistory(region, summoner, response.Games);
    else
        showResponseError(response);
}