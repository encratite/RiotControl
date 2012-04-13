function hasPrivilege(apiName)
{
    return system.privileges.indexOf(apiName) >= 0;
}

function hasSearchPrivilege()
{
    return hasPrivilege('Search');
}

function hasUpdatePrivilege()
{
    return hasPrivilege('Update');
}

function hasProfilePrivilege()
{
    return hasPrivilege('Profile');
}

function hasStatisticsPrivilege()
{
    return hasPrivilege('Statistics');
}

function hasGamesPrivilege()
{
    return hasPrivilege('Games');
}

function hasRunesPrivilege()
{
    return hasPrivilege('Runes');
}

function hasSetAutomaticUpdatesPrivilege()
{
    return hasPrivilege('SetAutomaticUpdates');
}