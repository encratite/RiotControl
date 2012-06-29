//Translating enumerated types

function getMapString(map)
{
    switch(map)
    {
    case 0:
        return 'Twisted Treeline';
    case 1:
        return "Summoner's Rift";
    case 2:
        return 'Dominion';
    case 3:
        return 'The Proving Grounds';
    default:
        return 'Unknown';
    }
}

function getGameModeString(mode)
{
    switch(mode)
    {
    case 0:
        return 'Custom';
    case 1:
        return 'Co-op vs. AI';
    case 2:
        return 'Normal';
    case 3:
        return 'Ranked Solo/Duo';
    case 4:
        return 'Ranked Teams';
    default:
        return 'Unknown';
    }
}