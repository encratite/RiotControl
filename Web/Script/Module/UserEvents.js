//Button/link handlers

function performSearch(description, summonerNameField, regionSelection, searchButton)
{
    setSearchFormState(summonerNameField, searchButton, false);

    var summonerName = summonerNameField.value;
    var regionSelect = regionSelection;
    var region = regionSelect.options[regionSelect.selectedIndex].value;
    if(description !== null)
    {
        description.purge();
        description.add('Searching for "' + summonerName + '"...');
    }
    setSearchRegion(region);
    apiFindSummoner(region, summonerName, function (response) { onSearchResult(response, region); } );
}

function updateSummoner(container, region, accountId)
{
    container.purge();
    container.add('Updating...');
    apiUpdateSummoner(region, accountId, function (response) { onSummonerUpdate(response, region, accountId); } );
}

function setAutomaticUpdates(container, region, summoner, enable)
{
    container.purge();
    container.add('Modifying settings...');
    apiSetAutomaticUpdates(region, summoner.AccountId, enable, function (response) { onSetAutomaticUpdates(response, container, region, summoner, enable); } );
}

function sortStatisticsAndRender(description, statistics, functionIndex, containerName)
{
    var container = document.getElementById(containerName);
    var isDescending;
    if (functionIndex == container.lastFunctionIndex)
        isDescending = !container.isDescending;
    else
    {
        var sortingFunctionData = getSortingFunctionData(functionIndex)
        isDescending = sortingFunctionData[1];
    }
    sortStatistics(statistics, functionIndex, isDescending);
    container.isDescending = isDescending;
    container.lastFunctionIndex = functionIndex;
    var statisticsTable = getStatisticsTable(description, statistics, containerName);
    container.purge();
    container.add(statisticsTable);
}