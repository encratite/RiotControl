set client_min_messages to warning;

drop table if exists summoner cascade;

create table summoner
(
        --PostgreSQL specific
        id serial primary key,

        region integer not null,

        account_id integer not null,
        summoner_id integer not null,

        summoner_name text not null,
        internal_name text not null,

        summoner_level integer not null,
        profile_icon integer not null,

        --Boolean value, indicates whether rating and match history data has ever been received for this summoner so far
        --Otherwise it's just an otherwise unconnected entry from a summoner search
        has_been_updated integer not null,

        --Boolean value
        update_automatically integer not null,

        --The time the profile was originally created (i.e. time of the first lookup)
        --UNIX timestamp, UTC
        time_created integer not null,

        --The last time the profile was last updated, UTC.
        --UNIX timestamp, UTC
        time_updated integer not null
);

--The following two indices might not be necessary because the stand-alone application caches summoner rows

--For lookups by account ID
create index summoner_account_id_index on summoner (region, account_id);

--PostgreSQL specific
--For lookups by name (case-insensitive)
create index summoner_summoner_name_index on summoner (region, lower(summoner_name));

--For the automatic updates
create index summoner_update_automatically_index on summoner (update_automatically);

drop table if exists summoner_rating cascade;

create table summoner_rating
(
        --PostgreSQL specific
        summoner_id integer references summoner(id) not null,

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
        top_rating integer
);

create index summoner_rating_summoner_id_index on summoner_rating (summoner_id);

--Required for updating irregular Elos below 1200 and also those in normal games
create index summoner_rating_update_index on summoner_rating (summoner_id, map, game_mode);

drop table if exists summoner_ranked_statistics cascade;

--This table holds the performance of a summoner with a particular champion in their ranked games.
--It is obtained from the ranked stats in their profile.
create table summoner_ranked_statistics
(
        --PostgreSQL specific
        summoner_id integer references summoner(id) not null,

        --A season of "0" indicates that these are stats for the current season
        season integer not null,

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
        maximum_deaths integer not null
);

create index summoner_ranked_statistics_summoner_id_index on summoner_ranked_statistics (summoner_id);

drop table if exists team cascade;

create table team
(
        --PostgreSQL specific
        id serial primary key
);

drop table if exists unknown_player cascade;

--This table holds superficial data about players who participated in a game but have not been loaded yet as this is very expensive
create table unknown_player
(
        --PostgreSQL specific
        team_id integer references team(id) not null,
        champion_id integer not null,
        summoner_id integer not null
);

drop table if exists game cascade;

create table game
(
        --PostgreSQL specific
        id serial primary key,

        --This is the internal game identifier used by the servers
        --Cannot be made unique because of the data from multiple regions being stored in the same table
        game_id integer not null,

        --Map type
        map integer not null,
        --Integer
        game_mode integer not null,

        --This is when the game was created.
        --UNIX timestamp, UTC
        time integer not null,

        --PostgreSQL specific
        blue_team_id integer references team (id) not null,
        --PostgreSQL specific
        purple_team_id integer  references team (id) not null,

        --Boolean value
        --0 if blue team won
        --1 if purple team won
        blue_team_won integer not null
);

--For sorting by time
create index game_time_index on game (time desc);

--For disjunctive lookups on team IDs
create index game_blue_team_id_index on game (blue_team_id);
create index game_purple_team_id_index on game (purple_team_id);

--Composite map/mode index
create index game_map_game_mode_index on game (map, game_mode);

drop table if exists player cascade;

--This table holds the results for one player in a game retrieved from the recent match history.
create table player
(
        --PostgreSQL specific
        game_id integer references game(id) not null,
        --PostgreSQL specific
        team_id integer references team(id) not null,
        --PostgreSQL specific
        summoner_id integer references summoner(id) not null,

        ping integer not null,
        time_spent_in_queue integer not null,

        premade_size integer not null,

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

        rank integer
);

create index player_game_id_index on player (game_id);
create index player_team_id_index on player (team_id);
create index player_summoner_id_index on player (summoner_id);

drop table if exists rune_page cascade;

create table rune_page
(
        --PostgreSQL specific
        id serial primary key,
        --PostgreSQL specific
        summoner_id integer references summoner(id) not null,
        name text not null,
        --Boolean value, indicates whether it is the rune page that is actually currently being used
        is_current_rune_page integer not null,
        --Time the rune page was created
        --UNIX timestamp, UTC
        time_created integer not null
);

create index rune_page_summoner_id_index on rune_page (summoner_id);

drop table if exists rune_slot cascade;

create table rune_slot
(
        --PostgreSQL specific
        rune_page_id integer references rune_page(id) on delete cascade not null,
        rune_slot integer not null,
        rune integer not null
);

create index rune_slot_rune_page_id_index on rune_slot (rune_page_id);