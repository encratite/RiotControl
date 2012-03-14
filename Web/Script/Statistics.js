function RankedStatistics(
    championName,

    wins,
    losses,

    kills,
    deaths,
    assists,

    minionKills,

    gold,

    turretsDestroyed,

    damageDealt,
    physicalDamageDealt,
    magicalDamageDealt,

    damageTaken,

    doubleKills,
    tripleKills,
    quadraKills,
    pentaKills,

    timeSpentDead,

    maximumKills,
    maximumDeaths
)
{
    this.championName = championName;

    this.wins = wins;
    this.losses = losses;

    this.kills = kills;
    this.deaths = deaths;
    this.assists = assists;

    this.minionKills = minionKills;

    this.gold = gold;

    this.turretsDestroyed = turretsDestroyed;

    this.damageDealt = damageDealt;
    this.physicalDamageDealt = physicalDamageDealt;
    this.magicalDamageDealt = magicalDamageDealt

    this.damageTaken = damageTaken;

    this.doubleKills = doubleKills;
    this.tripleKills = tripleKills;
    this.quadraKills = quadraKills;
    this.pentaKills = pentaKills;

    this.timeSpentDead = timeSpentDead;

    this.maximumKills = maximumKills;
    this.maximumDeaths = maximumDeaths;
}

function getTable()
{
    return document.getElementById('rankedStatistics');
}

var table = getTable();
table.originalHTML = table.innerHTML;