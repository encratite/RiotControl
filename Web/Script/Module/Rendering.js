//Common rendering functions

function render()
{
    var targets = [];
    for(var i in arguments)
        targets.push(arguments[i]);
    var newContent = getTemplate(targets)
    document.body.purge();
    document.body.add(newContent);
}

function renderWithoutTemplate()
{
    document.body.purge();
    for(var i in arguments)
        document.body.add(arguments[i]);
}

function loadingScreen()
{
    render(bold('Loading...'));
}

function loadIcon()
{
    var node = favicon('Icon/Icon.ico');
    document.head.add(node);
}

function loadStylesheet()
{
    var node = stylesheet('Style/Style.css');
    document.head.add(node);
}

function getTemplate()
{
    var logo = image('Logo.jpg', 'Riot Control', 1128, 157);
    logo.id = 'logo';

    var content = diverse();
    content.id = 'content';

    for(var i in arguments)
        content.add(arguments[i]);

    var output = [logo, content];
    return output;
}

function getRegionSelection()
{
    if(system.regionSelection === undefined)
    {
        var regionSelection = select('region');
        for(var i in system.regions)
        {
            var region = system.regions[i];
            var optionNode = option(region.description, region.abbreviation);
            regionSelection.add(optionNode);
        }
        system.regionSelection = regionSelection;
    }
    return system.regionSelection;
}

function getSearchForm(description)
{
    var summonerNameField = textBox();
    summonerNameField.id = 'summonerName';
    var regionSelection = getRegionSelection();
    var searchButton = submitButton('Search');
    var searchFunction = function() { performSearch(description, summonerNameField, regionSelection, searchButton); };
    searchButton.onclick = searchFunction;
    summonerNameField.onkeydown = function(event)
    {
        if(event.keyCode == 13 && !summonerNameField.disabled)
            searchFunction();
    };

    return [summonerNameField, regionSelection, searchButton];
}

function showError(message)
{
    showIndex(getErrorSpan(message));
}

function showResponseError(response)
{
    showError(getResultString(response));
}

function setSearchFormState(summonerNameField, searchButton, state)
{
    summonerNameField.disabled = !state;
    searchButton.disabled = !state;
}

function getErrorSpan(message)
{
    var node = span();
    node.className = 'error';
    node.add(message);
    return node;
}

function getTableHeadRow(fields)
{
    var output = tableRow();
    for(var i in fields)
    {
        var field = fields[i];
        var cell = tableHead();
        cell.add(field);
        output.add(cell);
    }
    return output;
}