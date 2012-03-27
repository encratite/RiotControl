//Utility functions

function trimString()
{
    return this.replace(/^\s+|\s+$/g, '');
}

//URL functions

function getURL(path)
{
    return system.baseURL + path;
}

function getBaseURL()
{
    var mainScript = document.getElementById('mainScript');
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
    element.clear = removeChildren;
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
        this.removeChild(node.lastChild);
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
    node.id = id;
    return node;
}

function submitButton(description, handler)
{
    var node = createElement('input');
    node.type = 'submit';
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

//Rendering/DOM functions

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
    
}

function getIndex()
{
    var description = paragraph();
    description.add('Enter the name of the summoner you want to look up:');
}