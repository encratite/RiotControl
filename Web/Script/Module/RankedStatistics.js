function RankedStatistics(statistics)
{
    //Inherit methods
    this.calculateExtendedStatistics = BasicStatistics.prototype.calculateExtendedStatistics;
    this.baseAdd = BasicStatistics.prototype.add;

    //Call base constructor
    this.base = BasicStatistics;
    this.base(statistics);

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
    this.baseAdd(statistics);

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