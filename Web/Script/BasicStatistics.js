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