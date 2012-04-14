function getStatisticsTable(description, statistics, containerName)
{
    if(statistics.length == 0)
        return '';

    var output = table();
    output.className = 'statistics';

    var tableCaption = caption(description);
    tableCaption.className = 'profile';
    output.add(tableCaption);

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
                [championDescription, false],
                [champion.gamesPlayed, true],
                [champion.wins, true],
                [champion.losses, true],
                [signum(champion.winLossDifference), true],
                [percentage(champion.wins / champion.gamesPlayed), true],
                [precisionString(champion.killsPerGame), true],
                [precisionString(champion.deathsPerGame), true],
                [precisionString(champion.assistsPerGame), true],
                [precisionString(champion.killsAndAssistsPerDeath), true],
                [precisionString(champion.minionKillsPerGame), true],
                [precisionString(champion.goldPerGame), true],
            ];

        var row = tableRow();
        if(champion.isSummary)
            row.className = 'allChampions';
        fields.forEach(function(field) {
            var content = field[0];
            var isNumeric = field[1];
            var cell = tableCell(content);
            if(isNumeric)
                cell.className = 'numeric';
            row.add(cell);
        });
        output.add(row);
    });

    return output;
}