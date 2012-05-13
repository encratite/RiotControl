create table rune_page
(
        id integer primary key,
        summoner_id integer not null,
        name text not null,
        is_current_rune_page integer not null,
        time_created integer not null,

        foreign key(summoner_id) references summoner(id)
);

create index rune_page_summoner_id_index on rune_page (summoner_id);