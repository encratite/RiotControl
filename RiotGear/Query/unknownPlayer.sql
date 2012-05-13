create table unknown_player
(
        team_id integer not null,
        champion_id integer not null,
        summoner_id integer not null,

        foreign key (team_id) references team(id)
);

create index unknown_player_team_id_index on unknown_player (team_id);