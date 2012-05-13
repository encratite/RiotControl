create table summoner
(
        id integer primary key,

        region integer not null,

        account_id integer not null,
        summoner_id integer not null,

        summoner_name text not null,
        internal_name text not null,

        summoner_level integer not null,
        profile_icon integer not null,

        has_been_updated integer not null,

        update_automatically integer not null,

        time_created integer not null,

        time_updated integer not null
);

create index summoner_account_id_index on summoner (region, account_id);

create index summoner_summoner_name_index on summoner (region, summoner_name collate nocase);

create index summoner_update_automatically_index on summoner (update_automatically);