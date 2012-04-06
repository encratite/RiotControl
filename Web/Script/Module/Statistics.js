function BasicStatistics(statistics)
{
    if(statistics !== undefined)
    {
        this.isSummary = false;
        this.name = getChampionName(statistics.ChampionId);

        this.wins = statistics.Wins;
        this.losses = statistics.Losses;

        this.kills = statistics.Kills;
        this.deaths = statistics.Deaths;
        this.assists = statistics.Assists;

        this.minionKills = statistics.MinionKills;

        this.gold = statistics.Gold;
    }
    else
    {
        this.isSummary = true;
        this.name = null;

        this.wins = 0;
        this.losses = 0;

        this.kills = 0;
        this.deaths = 0;
        this.assists = 0;

        this.minionKills = 0;

        this.gold = 0;
    }

    this.calculateExtendedStatistics();
}

BasicStatistics.prototype.calculateExtendedStatistics = function()
{
    var gamesPlayed = this.wins + this.losses;

    this.gamesPlayed = gamesPlayed;
    this.winLossDifference = this.wins - this.losses;

    this.killsPerGame = this.kills / gamesPlayed;
    this.deathsPerGame = this.deaths / gamesPlayed;
    this.assistsPerGame = this.assists / gamesPlayed;

    this.winRatio = this.wins / gamesPlayed;

    if(this.deaths > 0)
        this.killsAndAssistsPerDeath = (this.kills + this.assists) / this.deaths;
    else
        this.killsAndAssistsPerDeath = Infinity;

    this.minionKillsPerGame = this.minionKills / gamesPlayed;
    this.goldPerGame = this.gold / gamesPlayed;
};

BasicStatistics.prototype.add = function(statistics)
{
    this.wins += statistics.wins;
    this.losses += statistics.losses;

    this.kills += statistics.kills;
    this.deaths += statistics.deaths;
    this.assists += statistics.assists;

    this.minionKills += statistics.minionKills;

    this.gold += statistics.gold;

    this.calculateExtendedStatistics();
};

function RankedStatistics(statistics)
{
    //Call base constructor
    BasicStatistics.call(this, statistics);

    if(statistics !== undefined)
    {
        this.turretsDestroyed = statistics.TurretsDestroyed;

        this.damageDealt = statistics.DamageDealt;
        this.physicalDamageDealt = statistics.PhysicalDamageDealt;
        this.magicalDamageDealt = statistics.MagicalDamageDealt

        this.damageTaken = statistics.DamageTaken;

        this.doubleKills = statistics.DoubleKills;
        this.tripleKills = statistics.TripleKills;
        this.quadraKills = statistics.QuadraKills;
        this.pentaKills = statistics.PentaKills;

        this.timeSpentDead = statistics.TimeSpentDead;

        this.maximumKills = statistics.MaximumKills;
        this.maximumDeaths = statistics.MaximumDeaths;
    }
    else
    {
        this.turretsDestroyed = 0;

        this.damageDealt = 0;
        this.physicalDamageDealt = 0;
        this.magicalDamageDealt = 0

        this.damageTaken = 0;

        this.doubleKills = 0;
        this.tripleKills = 0;
        this.quadraKills = 0;
        this.pentaKills = 0;

        this.timeSpentDead = 0;

        this.maximumKills = 0;
        this.maximumDeaths = 0;
    }

    this.calculateExtendedStatistics();
}

RankedStatistics.prototype.add = function(statistics)
{
    BasicStatistics.prototype.add.call(this, statistics);

    this.turretsDestroyed += statistics.turretsDestroyed;

    this.damageDealt += statistics.damageDealt;
    this.physicalDamageDealt += statistics.physicalDamageDealt;
    this.magicalDamageDealt += statistics.magicalDamageDealt

    this.damageTaken += statistics.damageTaken;

    this.doubleKills += statistics.doubleKills;
    this.tripleKills += statistics.tripleKills;
    this.quadraKills += statistics.quadraKills;
    this.pentaKills += statistics.pentaKills;

    this.timeSpentDead += statistics.timeSpentDead;

    this.maximumKills = Math.max(this.maximumKills, statistics.maximumKills);
    this.maximumDeaths = Math.max(this.maximumDeaths, statistics.maximumDeaths);
}

RankedStatistics.prototype.calculateExtendedStatistics = BasicStatistics.prototype.calculateExtendedStatistics;