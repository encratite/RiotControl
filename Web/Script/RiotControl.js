function loadScript(url)
{
    var element = document.createElement('script')
    element.setAttribute('src', url);
}

function getURL(path)
{
    return getSourceURL() + path;
}

function isUndefined(input)
{
    return typeof input == 'undefined';
}

function module(path)
{
    var modulesLoaded = system.modulesLoaded;
    for(i in modulesLoaded)
    {
        if(modulesLoaded[i] == path)
        {
            //The module has already been loaded, return
            return;
        }
    }

    //It is a new module, load it and add it to the array of loaded modules
    modulesLoaded.push(path);
    loadScript(getURL('Script/' + path + '.js'));
}

function getBaseURL()
{
    var mainScript = document.getElementById('mainScript');
    if(isUndefined(mainScript))
        throw 'Unable to find the main script ID';
    var separator = '/';
    var tokens = mainScript.src.split(separator);
    if(tokens.length < 2)
        throw 'Invalid script path pattern';
    var baseURL = tokens.slice(0, -2).join(separator) + separator;
    return baseURL;
}

function System()
{
    //Name of this module
    this.modulesLoaded = ['Main'];
    this.baseURL = getBaseURL();
}

try
{
    system = new System();
}
catch(exception)
{
    alert('Initialisation error: ' + exception);
}