create table rune_slot
(
        rune_page_id integer not null,
        rune_slot integer not null,
        rune integer not null,

        foreign key(rune_page_id) references rune_page(id) on delete cascade
);

create index rune_slot_rune_page_id_index on rune_slot (rune_page_id);