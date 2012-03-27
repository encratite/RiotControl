//Utility functions

function trimString()
{
    return this.replace(/^\s+|\s+$/g, '');
}

//URL functions

function getURL(path)
{
    return getSourceURL() + path;
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

//main function

function main()
{
    try
    {
        String.prototype.trim = trimString;
        system = new System();
    }
    catch(exception)
    {
        alert('Initialisation error: ' + exception);
    }
}

//Markup-related functions

function htmlEscape(input)
{
    var output = html;
    var targets =
        [
            [/&/g, '&amp;'],
            [/</g, '&lt;'],
            [/>/g, '&gt;'],
            [/\"/g, '&quot;']
        ];
    for(var i in targets)
    {
        var target = targets[i];
        var pattern = target[0];
        var replacement = target[1];
        output = output.replace(pattern, replacement);
    }
    return output;
}


function getAttributeString(attributes)
{
    var output = '';
    for(var key in attributes)
    {
        var value = attributes[key];
        if(value == null)
            continue;
        if(key != 'href' && key != 'src')
            value = htmlEscape(value);
        output += ' ' + key + '="' + value + '"';
    }
    return output;
}

function emptyTag(tag, attributes)
{
    return '<' + tag + getAttributeString(attributes) + '>\n';
}

function tag(tag, content, attributes, innerNewlines)
{
    //Initialise default arguments
    if(attributes == undefined)
        attributes = {};
    if(innerNewlines == undefined)
        innerNewlines = true;

    if(content.Length == 0)
        innerNewlines = false;
    var newlineString = innerNewlines ? '\n' : '';
    return '<' + tag + getAttributeString(attributes) + '>' + newlineString + content.trim() + newlineString + '</' + tag + '>\n';
}

function paragraph(content, attributes)
{
    return tag('p', content, attributes);
}

function diverse(content, attributes)
{
    return tag('div', content, attributes);
}

function unorderedList(content, attributes)
{
    return tag('ul', content, attributes);
}

function listEntry(content, attributes)
{
    return tag('li', content, attributes, false);
}

function link(url, content, attributes)
{
    if(attributes == undefined)
        attributes = {};
    attributes.href = url;
    return tag('a', content, attributes, false);
}

function bold(content, attributes)
{
    return tag('b', attributes);
}

function span(content, attributes)
{
    return tag('span', content, attributes, false);
}

function image(uri, description, attributes)
{
    if(attributes == undefined)
        attributes = {};
    attributes.src = uri;
    attributes.alt = description;
    return emptyTag('img', attributes);
}

function table(content, attributes)
{
    return tag('table', content, attributes);
}

function tableRow(content, attributes)
{
    return tag('tr', content, attributes);
}

function tableCell(content, attributes)
{
    return tag('td', content, attributes, false);
}

function tableHead(content, attributes)
{
    return tag('th', content, attributes, false);
}

function caption(content, attributes)
{
    return tag('caption', content, attributes, false);
}

function form(action, content)
{
    if(attributes == undefined)
        attributes = {};
    attributes.onsubmit = action;
    return tag('form', content, attributes);
}

main();

alert(diverse('wefwef'));