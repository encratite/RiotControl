create table summoner_rating
(
        summoner_id integer not null,

        map integer not null,
        game_mode integer not null,

        wins integer not null,
        leaves integer not null,

        tier integer,
        rank integer,

        league_points integer,

        foreign key (summoner_id) references summoner(id)
);

create index summoner_rating_summoner_id_index on summoner_rating (summoner_id);

create index summoner_rating_update_index on summoner_rating (summoner_id, map, game_mode);

insert into summoner_rating (summoner_id, map, game_mode, wins, leaves, tier, rank, league_points) select summoner_id, map, game_mode, wins, leaves, null, null, null from old_summoner_rating;