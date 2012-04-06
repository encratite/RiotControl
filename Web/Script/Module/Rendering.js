//Common rendering functions

function render()
{
    var targets = [];
    parseArguments(arguments).forEach(function(argument) {
        targets.push(argument);
    });
    var newContent = getTemplate(targets)
    document.body.purge();
    document.body.add(newContent);
}

function renderWithoutTemplate()
{
    document.body.purge();
    parseArguments(arguments).forEach(function(argument) {
        document.body.add(argument);
    });
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

    parseArguments(arguments).forEach(function(argument) {
        content.add(argument);
    });

    var output = [logo, content];
    return output;
}

function getRegionSelection()
{
    if(system.regionSelection === undefined)
    {
        var searchRegion = getSearchRegion();
        var regionSelection = select('region');
        system.regions.forEach(function(region) {
            var optionNode = option(region.description, region.abbreviation);
            if(searchRegion !== null && region.abbreviation == searchRegion.abbreviation)
                optionNode.selected = 'selected';
            regionSelection.add(optionNode);
        });
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
    fields.forEach(function(field) {
        var cell = tableHead();
        cell.add(field);
        output.add(cell);
    });
    return output;
}