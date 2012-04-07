function getRunePageTitle(number, runePage)
{
    var title = runePage.Name;
    if(title.indexOf('@@!PaG3!@@') != -1)
        title = 'Rune Page ' + number;
    if(runePage.IsCurrentRunePage)
        title += ' (current)';

    return title;
}
function getSummonerRunePageTable(number, runePage)
{
    var output = table();
    output.className = 'runePage';
    output.id = runePage.anchorId;

    var tableCaption = caption(number + '. ' + runePage.title);
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
        var runeImage = image('Rune/' + rune.id + '.png', rune.name);
        var fields =
            {
                runeName: [runeImage, rune.name],
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

    if(runes.length == 0)
    {
        var cell = tableCell('This page is empty.');
        cell.colSpan = '3';
        cell.className = 'emptyRunePage';
        var row = tableRow(cell);
        output.add(row);
    }

    var cell = tableCell('Created: ' + getTimestampString(runePage.TimeCreated));
    cell.className = 'timeCreated';
    cell.colSpan = '3';
    var row = tableRow(cell);
    output.add(row);

    return output;
}

function renderSummonerRunes(summoner, runePages)
{
    log(summoner);
    var title = 'Runes of ' + summoner.SummonerName;
    setTitle(title);
    var header = header1(title);
    header.id = 'runePageHeader';
    var returnContainer = paragraph(anchor('Return to profile', function () { system.summonerHandler.open(getRegion(summoner.Region).abbreviation, summoner.AccountId); } ));
    returnContainer.id = 'returnFromRunes';
    var links = orderedList();
    var tables = [];
    runePages.reverse();
    runePages.forEach(function(runePage, i) {
        var number = i + 1;
        var runePage = runePages[i];
        runePage.title = getRunePageTitle(number, runePage);
        runePage.anchorId = 'page' + number;
        var link = anchor(runePage.title, function() { location.hash = '#' + runePage.anchorId; });
        if(runePage.IsCurrentRunePage)
            link.id = 'currentRunePageLink';
        links.add(listElement(link));
        tables.push(getSummonerRunePageTable(number, runePage))
    });
    var linksContainer = diverse(links);
    linksContainer.id = 'runePageLinks';
    render(header, returnContainer, linksContainer, tables);
}

function viewRunes(region, accountId)
{
    apiGetSummonerProfile(region, accountId, function (response) { onGetSummonerProfileForRunes(response, region); } );
}