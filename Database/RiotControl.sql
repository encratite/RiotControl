set client_min_messages to warning;

drop type if exists region_type cascade;

create type region_type as enum
(
        'north_america',
        'europe_west',
        'europe_nordic_east'
);

drop type if exists map_type cascade;

create type map_type as enum
(
        'twisted_treeline',
        'summoners_rift',
        'dominion'
);

drop type if exists game_mode_type cascade;

create type game_mode_type as enum
(
        'custom',
        'normal',
        'bot',
        'solo',
        'premade'
);

drop table if exists summoner cascade;

create table summoner
(
        id serial primary key,

        region region_type not null,

        --Cannot be made unique because of the data from multiple regions being stored in the same table
        account_id integer not null,
        summoner_id integer not null,

        summoner_name text not null,
        internal_name text not null,

        summoner_level integer not null,
        profile_icon integer not null,

        update_automatically boolean not null,

        --The time the profile was originally created (i.e. time of the first lookup), UTC.
        time_created timestamp not null,

        --The last time the profile was last updated, UTC.
        time_updated timestamp not null
);

create index summoner_summoner_name_index on summoner (lower(summoner_name));

drop table if exists summoner_rating cascade;

create table summoner_rating
(
        summoner_id integer references summoner(id) not null,

        rating_map map_type not null,
        game_mode game_mode_type not null,

        wins integer not null,
        losses integer not null,
        leaves integer not null,

        --May be null for normals
        current_rating integer,
        --top rating for unranked Summoner's Rift is estimated from all the values recorded, may be null for normals
        top_rating integer
);

create index summoner_rating_summoner_id_index on summoner_rating (summoner_id);

drop table if exists summoner_ranked_statistics cascade;

--This table holds the performance of a summoner with a particular champion in their ranked games.
--It is obtained from the ranked stats in their profile.
create table summoner_ranked_statistics
(
        summoner_id integer references summoner(id) not null,

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
        id serial primary key,
        --blue vs. purple, 100 is blue, 200 is purple
        is_blue_team boolean not null
);

drop table if exists missing_team_player cascade;

--This table holds superficial data about players who participated in a game but have not been loaded yet as this is very expensive
create table missing_team_player
(
        team_id integer references team(id) not null,
        champion_id integer not null,
        account_id integer not null
);

create index missing_team_player_index on missing_team_player (team_id, account_id);

drop table if exists game_result cascade;

create table game_result
(
        id serial primary key,

        --Cannot be made unique because of the data from multiple regions being stored in the same table
        game_id integer not null,

        result_map map_type not null,
        game_mode game_mode_type not null,

        --This is when the game was created.
        game_time timestamp not null,

        team1_won boolean not null,

        team1_id integer references team(id) not null,
        team2_id integer references team(id) not null
);

--create index game_result_team1_id_index on game_result (team1_id);
--create index game_result_team2_id_index on game_result (team2_id);
--create index game_result_map_mode_index on game_result (result_map, game_mode);

create index game_result_team1_index on game_result (result_map, game_mode, team1_id);
create index game_result_team2_index on game_result (result_map, game_mode, team2_id);

drop table if exists team_player cascade;

--This table holds the results for one player in a game retrieved from the recent match history.
create table team_player
(
        team_id integer references team(id) not null,
        summoner_id integer references summoner(id) not null,

        won boolean not null,

        ping integer not null,
        time_spent_in_queue integer not null,

        premade_size integer not null,

        --This is an argument used in the Elo formula
        k_coefficient integer not null,
        probability_of_winning double precision not null,

        --Elo may be left undefined as it is not available in custom games
        rating integer,
        rating_change integer,
        --I'm still not entirely sure what this one means
        adjusted_rating integer,

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

create index team_player_team_id_index on team_player (team_id);
create index team_player_summoner_id_index on team_player (summoner_id);

/*
drop table if exists champion_name cascade;

create table champion_name
(
        champion_id integer unique not null,
        champion_name text not null
);
*/