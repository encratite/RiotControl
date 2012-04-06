function getStatisticsTable(description, statistics, containerName)
{
    if(statistics.length == 0)
        return '';

    var output = table();

    output.className = 'statistics';
    output.add(caption(description));

    var columns =
        [
            'Champion',
            'Games',
            'W',
            'L',
            'W - L',
            'WR',
            'K',
            'D',
            'A',
            '(K + A) / D',
            'MK',
            'Gold',
        ];

    var row = tableRow();
    for(var i = 0; i < columns.length; i++)
    {
        var link = anchor(columns[i], getSortableColumnFunction(description, statistics, i, containerName));
        row.add(tableHead(link));
    }
    output.add(row);

    statistics.forEach(function(champion) {
        var name;
        var image;
        if(champion.isSummary)
        {
            name = 'All champions';
            image = 'Placeholder';
        }
        else
        {
            name = champion.name;
            image = encodeURI(champion.name);
        }

        var championDescription =
            [
                icon('Champion/Small/' + image + '.png', name),
                name,
            ];

        var fields =
            [
                championDescription,
                champion.gamesPlayed,
                champion.wins,
                champion.losses,
                signum(champion.winLossDifference),
                percentage(champion.wins / champion.gamesPlayed),
                precisionString(champion.killsPerGame),
                precisionString(champion.deathsPerGame),
                precisionString(champion.assistsPerGame),
                precisionString(champion.killsAndAssistsPerDeath),
                precisionString(champion.minionKillsPerGame),
                precisionString(champion.goldPerGame),
            ];

        var row = tableRow();
        if(champion.isSummary)
            row.className = 'allChampions';
        fields.forEach(function(field) {
            row.add(tableCell(field));
        });
        output.add(row);
    });

    return output;
}