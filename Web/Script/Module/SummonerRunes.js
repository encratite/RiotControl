function getSummonerRunePageTable(number, runePage)
{
    var output = table();
    output.className = 'runePage';

    var title = '[' + number + '] ' + runePage.Name;
    if(runePage.IsCurrentRunePage)
        title += ' (current)';
    var tableCaption = caption(title);
    tableCaption.className = 'runePage';
    output.add(tableCaption);

    var titles =
        [
            'Rune',
            'Description',
            'Count',
        ];

    output.add(getTableHeadRow(titles));

    var runeCounters = {};
    runePage.Slots.forEach(function(slot) {
        var key = slot.Rune;
        if(key in runeCounters)
            runeCounters[key]++;
        else
            runeCounters[key] = 1;
    });
    var runes = [];
    for(var id in runeCounters)
    {
        id = parseInt(id);
        var count = runeCounters[id];
        var rune = getRune(id);
        rune.id = id;
        rune.count = count;
        runes.push(rune);
    }
    runes.sort(function(x, y) {
        x.id - y.id;
    });
    runes.forEach(function(rune) {
        var fields =
            {
                runeName: [image('Rune/' + rune.id + '.png', rune.name), rune.name],
                runeDescription: rune.description,
                runeCount: rune.count.toString(),
            };
        var row = tableRow();
        for(var field in fields)
        {
            var cell = tableCell(fields[field]);
            cell.className = field;
            row.add(cell);
        }
        output.add(row);
    });

    var cell = tableCell('Created: ' + getTimestampString(runePage.TimeCreated));
    cell.className = 'timeCreated';
    cell.colSpan = '3';
    var row = tableRow(cell);
    output.add(row);

    return output;
}

function renderSummonerRunes(summoner, runePages)
{
    setTitle('Runes of ' + summoner.SummonerName);
    var tables = [];
    for(var i = 0; i < runePages.length; i++)
    {
        var number = i + 1;
        var runePage = runePages[i];
        tables.push(getSummonerRunePageTable(number, runePage))
    }
    render(tables);
}

function viewRunes(region, accountId)
{
    apiGetSummonerProfile(region, accountId, function (response) { onGetSummonerProfileForRunes(response, region); } );
}