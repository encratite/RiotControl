//Riot Control JSON API

function apiFindSummoner(region, summonerName, callback)
{
    apiCall('Search', [region, summonerName], callback);
}

function apiUpdateSummoner(region, accountId, callback)
{
    apiCall('Update', [region, accountId], callback);
}

function apiGetSummonerProfile(region, accountId, callback)
{
    apiCall('Profile', [region, accountId], callback);
}

function apiGetSummonerStatistics(region, accountId, callback)
{
    apiCall('Statistics', [region, accountId], callback);
}

function apiGetMatchHistory(region, accountId, callback)
{
    apiCall('Games', [region, accountId], callback);
}

function apiGetSummonerRunes(region, accountId, callback)
{
    apiCall('Runes', [region, accountId], callback);
}

function apiSetAutomaticUpdates(region, accountId, enable, callback)
{
    apiCall('SetAutomaticUpdates', [region, accountId, enable ? 1 : 0], callback);
}