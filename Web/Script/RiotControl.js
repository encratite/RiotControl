//Utility functions

function trimString()
{
    return this.replace(/^\s+|\s+$/g, '');
}

function apiCall(name, callArguments, callback)
{
    var path = '/API/' + name;
    for(var i in callArguments)
    {
        var argument = callArguments[i];
        path += '/' + argument;
    }
    var request = new XMLHttpRequest();
    request.onreadystatechange = function()
    {
        if(request.readyState == 4)
        {
            if(request.status == 200)
            {
                var response = JSON.parse(request.responseText);
                callback(response);
            }
            else
                throw 'API error in ' + path + ': ' + request.responseText;
        }
    }
    request.open('GET', path);
    request.send();
}

function getById(id)
{
    return document.getElementById(id);
}

//URL functions

function getURL(path)
{
    return system.baseURL + path;
}

function getBaseURL()
{
    var mainScript = getById('mainScript');
    if(mainScript == undefined)
        throw 'Unable to find the main script ID';
    var separator = '/';
    var tokens = mainScript.src.split(separator);
    if(tokens.length < 2)
        throw 'Invalid script path pattern';
    var baseURL = tokens.slice(0, -2).join(separator) + separator;
    return baseURL;
}

//Region class

function Region(abbreviation, description)
{
    this.abbreviation = abbreviation;
    this.description = description;
}

//Global initialisation

function initialiseSystem(regions, privileged)
{
    system = {};
    system.baseURL = getBaseURL();
    system.content = getTemplate()
    system.privileged = privileged;
    system.regions = [];
    for(i in regions)
    {
        var info = regions[i];
        var abbreviation = info[0];
        var description = info[1];
        var region = new Region(abbreviation, description);
        system.regions.push(region);
    }
}

function initialise(regions, privileged)
{
    initialiseSystem(regions, privileged);
    installStringExtensions();
    loadStylesheet();
    render(getIndex());
}

function installStringExtensions()
{
    String.prototype.trim = trimString;
}

//Content generation

function createElement(tag)
{
    var element = document.createElement(tag);
    //Extensions
    element.add = addChild;
    element.purge = removeChildren;
    return element;
}

function addChild(input)
{
    if(typeof input == 'string')
        input = text(input);
    this.appendChild(input);
}

function removeChildren()
{
    while(this.hasChildNodes())
        this.removeChild(this.lastChild);
}

function text(text)
{
    var element = document.createTextNode(text);
    return element;
}

function image(path, description)
{
    var image = createElement('img');
    image.src = getURL('Image/' + path);
    image.alt = description;
    return image;
}

function diverse()
{
    return createElement('div');
}

function paragraph()
{
    return createElement('p');
}

function link(relationship, type, reference)
{
    var node = createElement('link');
    node.rel = relationship;
    node.type = type;
    node.href = reference;
    return node;
}

function stylesheet(path)
{
    return link('stylesheet', 'text/css', getURL(path));
}

function textBox(id)
{
    var node = createElement('input');
    node.type = 'text';
    node.className = 'text';
    node.id = id;
    return node;
}

function submitButton(description, handler)
{
    var node = createElement('input');
    node.type = 'submit';
    node.className = 'submit';
    node.value = description;
    node.onclick = handler;
    return node;
}

function select(id)
{
    var node = createElement('select');
    node.id = id;
    return node;
}

function option(description, value)
{
    var node = createElement('option');
    node.value = value;
    node.add(description);
    return node;
}

function span()
{
    return createElement('span');
}

//Rendering/DOM functions

function render(node)
{
    system.content.purge();
    system.content.add(node);
}

function loadStylesheet()
{
    var node = stylesheet('Style/Style.css');
    document.head.appendChild(node);
}

function getTemplate()
{
    var logo = image('Logo.jpg');
    logo.id = 'logo';

    var content = diverse();
    content.id = 'content';

    document.body.appendChild(logo);
    document.body.appendChild(content);

    return content;
}

function getRegionSelection()
{
    var selectNode = select('region');
    for(var i in system.regions)
    {
        var region = system.regions[i];
        var optionNode = option(region.description, region.abbreviation);
        selectNode.add(optionNode);
    }
    return selectNode;
}

function getIndex()
{
    var container = diverse();
    container.id = 'indexForm';

    var description = paragraph();
    description.id = 'searchDescription';
    description.add('Enter the name of the summoner you want to look up:');
    var summonerNameField = textBox('summonerName');
    var regionSelection = getRegionSelection();
    var button = submitButton('Search', performSearch);
    button.id = 'searchButton';

    container.add(description);
    container.add(summonerNameField);
    container.add(regionSelection);
    container.add(button);

    return container;
}

function setSearchFormState(state)
{
    var textBox = getById('summonerName');
    textBox.disabled = !state;
    var button = getById('searchButton');
    button.disabled = !state;
}

function searchError(message)
{
    var node = span();
    node.className = 'error';
    node.add(message);
    setSearchDescription(node);
    setSearchFormState(true);
}

function setSearchDescription(node)
{
    var paragraph = getById('searchDescription');
    paragraph.purge();
    paragraph.add(node);
}

//Event handlers

function performSearch()
{
    setSearchFormState(false);

    var summonerName = getById('summonerName').value;
    var regionSelect = getById('region');
    var region = regionSelect.options[regionSelect.selectedIndex].value;
    setSearchDescription('Searching for "' + summonerName + '"...');
    apiCall('Search', [region, summonerName], processSearchResult);
}

function processSearchResult(response)
{
    var result = response.Result;
    if(result == 'Success')
    {
        alert('Success: ' + response.AccountId);
    }
    else if(result == 'NotFound')
        searchError('Unable to find summoner.');
    else if(result == 'Timeout')
        searchError('The request has timed out.');
    else if(result == 'NotConnected')
        searchError('The worker for this region is not connected.');
}