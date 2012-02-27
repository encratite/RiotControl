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