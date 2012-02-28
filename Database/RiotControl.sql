drop table if exists summoner cascade;

create table summoner
(
        id serial primary key,

        account_id integer unique not null,
        summoner_id integer unique not null,
        summoner_name text not null,
        internal_name text not null,
        summoner_level integer not null,
        profile_icon integer not null
);

drop type map_type;

create type if exists map_type as enum
(
        'twisted_treeline',
        'summoners_rift',
        'dominion'
);

drop type if exists queue_mode_type;

create type queue_mode_type as enum
(
        'custom',
        'normal',
        'solo',
        'premade'
);

drop table if exists summoner_rating cascade;

create table summoner_rating
(
        summoner_id integer references summoner(id) not null,
        rating_map map_type not null,
        queue_mode queue_mode_type not null,
        current_rating integer not null,
        --top rating for unranked Summoner's Rift is estimated from all the values recorded
        top_rating integer not null
);

drop table if exists summoner_ranked_statistics cascade;

--This table holds the performance of a summoner with a particular champion in their ranked games.
--It is obtained from the ranked stats in their profile.
create table summoner_ranked_statistics
(
        summoner_id integer references summoner(id) not null,

        champion_id integer not null,

        victories integer not null,
        defeats integer not null,

        kills integer not null,
        deaths integer not null,
        assists integer not null,

        minion_kills integer not null,

        gold integer not null,

        turrets_destroyed integer not null,

        damage_dealt integer not null,
        physical_damage_dealt integer not null,
        magical_damage_dealt integer not null,

        damage_taken integer not null,

        double_kills integer not null,
        triple_kills integer not null,
        quadra_kills integer not null,
        penta_kills integer not null,

        time_spent_dead integer not null,

        maximum_kills integer not null,
        maximum_deaths integer not null
);

drop table if exists team cascade;

create table team
(
        id serial primary key
);

drop table if exists game_result cascade;

create table game_result
(
        id serial primary key,

        result_map map_type not null,
        queue_mode queue_mode_type not null,

        team1_won boolean not null,

        team1_id integer references team(id) not null,
        team2_id integer references team(id) not null
);

drop table if exists team_player cascade;

--This table holds the results for one player in a game retrieved from the recent match history.
create table team_player
(
        team_id integer references team(id) not null,
        summoner_id integer references summoner(id) not null,

        --Elo may be left undefined as it is not available in custom games
        rating integer,
        rating_change integer,

        champion_id integer not null,

        champion_level integer not null,

        items integer array[6] not null,

        kills integer not null,
        deaths integer not null,
        assists integer not null,

        minion_kills integer not null,
        neutral_minions_killed integer not null,

        gold integer not null,

        turrets_destroyed integer not null,
        inhibitors_destroyed integer not null,

        damage_dealt integer not null,
        physical_damage_dealt integer not null,
        magical_damage_dealt integer not null,

        damage_taken integer not null,
        physical_damage_taken integer not null,
        magical_damage_taken integer not null,

        total_healing_done integer not null,

        time_spent_dead integer not null,

        largest_multikill integer not null,
        largest_killing_spree integer not null,
        largest_critical_strike integer not null
);

drop table if exists champion_statistics;

--This table holds the cumulative statistics of a particular champion in a particular game mode for a particular rating range.
create table champion_statistics
(
        id serial primary key,

        rating_map map_type not null,
        queue_mode queue_mode_type not null,

        champion_id integer not null,

        --Rating boundaries are both set to NULL if the statistics are for all ratings instead of just a smaller range
        minimum_rating integer,
        maximum_rating integer,

        victories integer not null,
        defeats integer not null,

        kills integer not null,
        deaths integer not null,
        assists integer not null,

        minion_kills integer not null,

        gold integer not null,

        turrets_destroyed integer not null,

        damage_dealt integer not null,
        physical_damage_dealt integer not null,
        magical_damage_dealt integer not null,

        damage_taken integer not null,

        time_spent_dead integer not null,
);