/*
Enumerations:

Regions:

NorthAmerica = 0,
EuropeWest = 1,
EuropeNordicEast = 2,

Maps:

TwistedTreeline = 0,
SummonersRift = 1,
Dominion = 2,

Game modes:

Custom = 0,
Normal = 1,
Bot = 2,
Solo = 3,
Premade = 4,
*/

--Activate foreign key support in SQLite
pragma foreign_keys = on;

drop table if exists summoner;

create table summoner
(
        id integer primary key,

        region integer not null,

        --Cannot be made unique because of the data from multiple regions being stored in the same table
        account_id integer not null,
        summoner_id integer not null,

        summoner_name text not null,
        internal_name text not null,

        summoner_level integer not null,
        profile_icon integer not null,

        --Boolean value
        update_automatically integer not null,

        --The time the profile was originally created (i.e. time of the first lookup)
        --UNIX timestamp, UTC
        time_created integer not null,

        --The last time the profile was last updated, UTC.
        --UNIX timestamp, UTC
        time_updated integer not null
);

--For lookups by account ID
create index summoner_account_id_index on summoner (region, account_id);

--For lookups by name (case-insensitive)
create index summoner_summoner_name_index on summoner (region, summoner_name collate nocase);

--For the automatic updates
create index summoner_update_automatically_index on summoner (update_automatically);

drop table if exists summoner_rating;

create table summoner_rating
(
        summoner_id integer not null,

        --Map type
        map integer not null,
        --Game mode
        game_mode integer not null,

        wins integer not null,
        losses integer not null,
        leaves integer not null,

        --May be null for normals
        current_rating integer,
        --top rating for unranked Summoner's Rift is estimated from all the values recorded, may be null for normals
        top_rating integer,

        foreign key (summoner_id) references summoner(id)
);

create index summoner_rating_summoner_id_index on summoner_rating (summoner_id);

--Required for updating irregular Elos below 1200 and also those in normal games
create index summoner_rating_update_index on summoner_rating (summoner_id, map, game_mode);

drop table if exists summoner_ranked_statistics;

--This table holds the performance of a summoner with a particular champion in their ranked games.
--It is obtained from the ranked stats in their profile.
create table summoner_ranked_statistics
(
        summoner_id integer not null,

        champion_id integer not null,

        wins integer not null,
        losses integer not null,

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
        maximum_deaths integer not null,

        foreign key (summoner_id) references summoner(id)
);

create index summoner_ranked_statistics_summoner_id_index on summoner_ranked_statistics (summoner_id);

drop table if exists team;

create table team
(
        id integer primary key
);

drop table if exists unknown_player;

--This table holds superficial data about players who participated in a game but have not been loaded yet as this is very expensive
create table unknown_player
(
        team_id integer not null,
        champion_id integer not null,
        account_id integer not null,

        foreign key (team_id) references team(id)
);

drop table if exists game;

create table game
(
        id integer primary key,

        --This is the internal game identifier used by the servers
        --Cannot be made unique because of the data from multiple regions being stored in the same table
        game_id integer not null,

        --Map type
        map integer not null,
        --Integer
        game_mode integer not null,

        --This is when the game was created.
        --UNIX timestamp, UTC
        time timestamp not null,

        --If blue team won, the value is 0
        --If purple team won, the value is 1
        winner integer not null,

        blue_team_id integer not null,
        purple_team_id integer  not null,

        foreign key (blue_team_id) references team (id),
        foreign key (purple_team_id) references team (id)
);

--For sorting by time
create index game_time_index on game (time desc);

--For disjunctive lookups on team IDs
create index game_blue_team_id_index on game (blue_team_id);
create index game_purple_team_id_index on game (purple_team_id);

--Composite map/mode index
create index game_map_game_mode_index on game (map, game_mode);

drop table if exists player;

--This table holds the results for one player in a game retrieved from the recent match history.
create table player
(
        game_id integer null,
        team_id integer not null,
        summoner_id integer not null,

        won boolean not null,

        ping integer not null,
        time_spent_in_queue integer not null,

        premade_size integer not null,

        --This is an argument used in the Elo formula
        k_coefficient integer not null,
        probability_of_winning real not null,

        --Elo may be left undefined as it is not available in custom games
        rating integer,
        rating_change integer,
        --I'm still not entirely sure what this one means
        adjusted_rating integer,
        team_rating integer,

        experience_earned integer not null,
        boosted_experience_earned integer not null,

        ip_earned integer not null,
        boosted_ip_earned integer not null,

        summoner_level integer not null,

        summoner_spell1 integer not null,
        summoner_spell2 integer not null,

        champion_id integer not null,

        --can be NULL, apparently
        skin_name text,
        skin_index integer not null,

        champion_level integer not null,

        items integer array[6] not null,

        kills integer not null,
        deaths integer not null,
        assists integer not null,

        minion_kills integer not null,

        gold integer not null,

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
        largest_critical_strike integer not null,

        --Summoner's Rift/Twisted Treeline

        neutral_minions_killed integer,

        turrets_destroyed integer,
        inhibitors_destroyed integer,

        --Dominion

        nodes_neutralised integer,
        node_neutralisation_assists integer,
        nodes_captured integer,

        victory_points integer,
        objectives integer,

        total_score integer,
        objective_score integer,
        combat_score integer,

        rank integer,

        foreign key(game_id) references game(id),
        foreign key(team_id) references team(id),
        foreign key(summoner_id) references summoner(id)
);

create index player_game_id_index on player (game_id);
create index player_team_id_index on player (team_id);
create index player_summoner_id_index on player (summoner_id);

drop table if exists champion_name;

--No explicit indices are provided for the following two tables as they are just loaded once when the application starts
--After that, the application performs the translation itself because it's probably faster and a very common operation

create table champion_name
(
        champion_id integer unique not null,
        champion_name text not null
);

drop table if exists item_information;

create table item_information
(
        item_id integer unique not null,
        item_name text not null,
        description text not null
);

vacuum;