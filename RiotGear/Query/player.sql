create table player
(
        game_id integer not null,
        team_id integer not null,
        summoner_id integer not null,

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