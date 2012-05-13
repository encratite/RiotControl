create table summoner_rating
(
        summoner_id integer not null,

        map integer not null,
        game_mode integer not null,

        wins integer not null,
        losses integer not null,
        leaves integer not null,

        current_rating integer,
        top_rating integer,

        foreign key (summoner_id) references summoner(id)
);

create index summoner_rating_summoner_id_index on summoner_rating (summoner_id);

create index summoner_rating_update_index on summoner_rating (summoner_id, map, game_mode);