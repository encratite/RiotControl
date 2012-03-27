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

//System class, contains data relevant to all of the application

function System()
{
    this.baseURL = getBaseURL();
}

//Global initialisation

function initialise()
{
    try
    {
        String.prototype.trim = trimString;
        system = new System();
    }
    catch(exception)
    {
        alert('Initialisation error: ' + exception);
        return;
    }

    loadStylesheet();
    renderTemplate();
}

//Content generation

function createElement(tag)
{
    var element = document.createElement(tag);
    element.add = element.appendChild;
    return element;
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

function link(relationship, type, reference)
{
    var link = createElement('link');
    link.rel = relationship;
    link.type = type;
    link.href = reference;
    return link;
}

function stylesheet(path)
{
    return link('stylesheet', 'text/css', getURL(path));
}

//Rendering/DOM functions

function loadStylesheet()
{
    var node = stylesheet('Style/Style.css');
    document.head.appendChild(node);
}

function renderTemplate()
{
    var logo = image('Logo.jpg');
    logo.id = 'logo';

    var content = diverse();
    content.id = 'content';

    content.add(text('Test'));

    document.body.appendChild(logo);
    document.body.appendChild(content);
}

initialise();