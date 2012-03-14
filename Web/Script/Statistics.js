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

    var gamesPlayed = wins + losses;

    this.gamesPlayed = gamesPlayed;

    this.killsPerGame = kills / gamesPlayed;
    this.deathsPerGame = deaths / gamesPlayed;
    this.assistsPerGame = assists / gamesPlayed;

    if(deaths > 0)
        this.killsAndAssistsPerDeath = (kills + assists) / deaths;
    else
        this.killsAndAssistsPerDeath = '∞';

    this.minionKillsPerGame = minionKills / gamesPlayed;
    this.goldPerGame = gold / gamesPlayed;
}

function getContainer()
{
    return document.getElementById('rankedStatistics');
}

function getCell(contents)
{
    return "<td>" + contents + "</td>\n";
}

function getHeadCell(contents)
{
    return "<th>" + contents + "</th>\n";
}

function getSpan(styleClass, content)
{
    return '<span class="' + styleClass + '">' + content + '</span>'
}

function getSignumString(number)
{
    var styleClass;
    if(number > 0)
    {
        styleClass = 'positive';
        number = '+' + number;
    }
    else if(number == 0)
        return '±' + number;
    else
        styleClass = 'negative';
    return getSpan(styleClass, number);
}

function getPercentage(input)
{
    return (input * 100).toFixed(1) + '%';
}

function getPrecisionString(input)
{
    if(typeof input == 'string')
        return input;
    else
        return input.toFixed(1);
}

function getChampionStatisticsRow(statistics)
{
    var fields =
        [
            '<img src="/RiotControl/Static/Image/Champion/Small/' + encodeURI(statistics.championName) + '.png" alt="' + statistics.championName + '">' + statistics.championName + '</a>',
            statistics.gamesPlayed,
            statistics.wins,
            statistics.losses,
            getSignumString(statistics.wins - statistics.losses),
            getPercentage(statistics.wins / statistics.gamesPlayed),
            getPrecisionString(statistics.killsPerGame),
            getPrecisionString(statistics.deathsPerGame),
            getPrecisionString(statistics.assistsPerGame),
            getPrecisionString(statistics.killsAndAssistsPerDeath),
            getPrecisionString(statistics.minionKillsPerGame),
            getPrecisionString(statistics.goldPerGame),
        ]

    var output = "<tr>\n";
    for(var i in fields)
        output += getCell(fields[i]);
    output += "</tr>\n";
    return output;
}

function writeTable(rankedStatistics)
{
    var markup = "<table>\n";
    markup += "<caption>Ranked Statistics</caption>\n";
    var columns =
        [
            'Champion',
            'Games',
            'W',
            'L',
            'W - L',
            'WR',
            'K',
            'D',
            'A',
            '(K + A) / D',
            'MK',
            'Gold',
        ];
    markup += "<tr>\n";
    for(var i in columns)
        markup += getHeadCell(columns[i]);
    markup += "</tr>\n";
    for(var i in rankedStatistics)
        markup += getChampionStatisticsRow(rankedStatistics[i]);
    markup += "</table>\n";
    var container = getContainer();
    container.innerHTML = markup;
}