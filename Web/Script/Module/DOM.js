//Content generation

function createElement(tag, children)
{
    var element = document.createElement(tag);
    //Extensions
    element.add = addChild;
    element.purge = removeChildren;
    if(children !== undefined)
    {
        children.forEach(function(child) {
            element.add(child);
        });
    }
    return element;
}

function addChild(input)
{
    if(input === undefined)
    {
        trace();
        throw 'Tried to add an undefined component';
    }
    if(typeof input == 'string')
        input = text(input);
    else if(typeof input == 'number')
        input = text(input.toString());
    else if(input.isArray)
    {
        var container = this;
        input.forEach(function(i) {
            container.add(i);
        });
        return;
    }
    try
    {
        this.appendChild(input);
    }
    catch(exception)
    {
        //Firebug only
        if(console !== undefined)
            console.trace();
        throw exception;
    }
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

function getPixels(pixels)
{
    return pixels + 'px';
}

function image(path, description, width, height)
{
    var image = createElement('img');
    image.src = getURL('Image/' + path);
    image.alt = description;
    if(width !== undefined && height !== undefined)
    {
        image.style.width = getPixels(width);
        image.style.height = getPixels(height);
    }
    return image;
}

function icon(path, description)
{
    var output = image(path, description, 32, 32);
    return output;
}

function diverse()
{
    return createElement('div', parseArguments(arguments));
}

function paragraph()
{
    return createElement('p', parseArguments(arguments));
}

function link(relationship, type, reference)
{
    var node = createElement('link');
    node.rel = relationship;
    node.type = type;
    node.href = reference;
    return node;
}

function favicon(path)
{
    return link('icon', 'image/ico', getURL(path));
}

function stylesheet(path)
{
    return link('stylesheet', 'text/css', getURL(path));
}

function textBox()
{
    var node = createElement('input');
    node.type = 'text';
    node.className = 'text';
    return node;
}

function submitButton(description)
{
    var node = createElement('input');
    node.type = 'submit';
    node.className = 'submit';
    node.value = description;
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
    return createElement('span', parseArguments(arguments));
}

function bold()
{
    return createElement('b', parseArguments(arguments));
}

function anchor(description, handler)
{
    var node = createElement('a');
    node.onclick = handler;
    node.add(description);
    return node;
}

function caption(title)
{
    var node = createElement('caption');
    node.add(title);
    return node;
}

function table()
{
    return createElement('table', parseArguments(arguments));
}

function tableRow()
{
    return createElement('tr', parseArguments(arguments));
}

function tableCell()
{
    return createElement('td', parseArguments(arguments));
}

function tableHead()
{
    return createElement('th', parseArguments(arguments));
}

function list()
{
    return createElement('ul', parseArguments(arguments));
}

function orderedList()
{
    return createElement('ol', parseArguments(arguments));
}

function listElement()
{
    return createElement('li', parseArguments(arguments));
}

function header1()
{
    return createElement('h1', parseArguments(arguments));
}

function header2()
{
    return createElement('h2', parseArguments(arguments));
}

function header3()
{
    return createElement('h3', parseArguments(arguments));
}
