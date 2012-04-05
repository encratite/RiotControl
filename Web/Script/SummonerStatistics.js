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
    for(var i in columns)
    {
        var column = columns[i];
        var link = anchor(column, getSortableColumnFunction(description, statistics, i, containerName));
        row.add(tableHead(link));
    }
    output.add(row);

    for(var i in statistics)
    {
        var champion = statistics[i];

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
        for(var i in fields)
            row.add(tableCell(fields[i]));
        output.add(row);
    }

    return output;
}