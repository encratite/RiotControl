function getSummonerSpell(id)
{
    switch(id)
    {
    case 1:
        return 'Cleanse';
    case 2:
        return 'Clairvoyance';
    case 3:
        return 'Exhaust';
    case 4:
        return 'Flash';
    case 6:
        return 'Ghost';
    case 7:
        return 'Heal';
    case 10:
        return 'Revive';
    case 11:
        return 'Smite';
    case 12:
        return 'Teleport';
    case 13:
        return 'Clarity';
    case 14:
        return 'Ignite';
    case 16:
        return 'Surge';
    case 17:
        return 'Garrison';
    case 20:
        return 'Promote';
    default:
        return 'Unknown';
    }
}