//System initialisation

function getModules()
{
    var modules =
    [
        'ChampionNames',
        'Items',
        'Runes',

        'Extensions',
        'Utility',

        'Classes',
        'Statistics',

        'Handlers',

        'API',

        'DOM',
        'Rendering',

        'UserEvents',
        'APIEvents',

        'Enumeration',

        'Index',

        'SummonerOverview',
        'SummonerProfile',
        'SummonerRating',
        'SummonerStatistics',
        'SummonerGames',
        'SummonerRunes',
    ];

    return modules;
}

function initialiseSystem(regions, privileged, revision, moduleMode)
{
    system = {};
    system.baseURL = getBaseURL(moduleMode);
    system.privileged = privileged;
    system.revision = revision;
    system.regions = [];
    for(var i = 0; i < regions.length; i++)
    {
        var information = regions[i];
        var abbreviation = information[0];
        var description = information[1];
        var identifier = information[2];
        var region = new Region(abbreviation, description, identifier);
        system.regions[identifier] = region;
    }
}

function initialise(regions, privileged, revision, moduleMode)
{
    if(moduleMode === undefined)
        moduleMode = false;
    initialiseSystem(regions, privileged, revision, moduleMode);
    if(moduleMode)
        loadModules(getModules(), runSystem);
    else
        runSystem();
}

function revisionCheck()
{
    var oldestRevisionSupported = 282;
    var please = ' Please update your software.'
    //Make sure that a system.revision of 0 always passes the revision check as it is the value used by bleeding edge builds where users where too lazy to enable generateAssemblyInfo
    if(system.revision === undefined || system.revision === null)
    {
        alert('Your Riot Control client is outdated. You need at least r' + oldestRevisionSupported + ' to use this system.' + please);
        return false;
    }
    else if(system.revision > 0 && system.revision < oldestRevisionSupported)
    {
        alert('You are running Riot Control r' + system.revision + ' but you need at least r' + oldestRevisionSupported + ' to use this system.' + please);
        return false;
    }
    else
        return true;
}

function runSystem()
{
    if(!revisionCheck())
    {
        location.href = 'http://riot.control-c.ir/';
        return;
    }
    installExtensions();
    loadIcon();
    loadStylesheet();
    installHandlers();
    routeRequest();
}

function routeRequest()
{
    var request = getRequest();
    if(request === null)
    {
        system.defaultHandler();
        return;
    }
    var handlers = system.requestHandlers;
    for(var i = 0; i < handlers.length; i++)
    {
        var handler = handlers[i];
        if(handler.name == request.name)
        {
            handler.execute(request.requestArguments);
            return;
        }
    }
    showError('Unknown request path specified.');
}

//Module management

function loadModules(modules, callback)
{
    var remainingModules = modules.slice();
    modules.forEach(function(module) {
        var script = document.createElement('script');
        script.onerror = function() { alert('Unable to load module "' + module + '".'); }
        script.onload = function(script) { moduleOnLoad(script, remainingModules, callback); };
        script.src = getURL('Script/Module/' + module + '.js');
        document.body.appendChild(script);
    });
}

function moduleOnLoad(script, remainingModules, callback)
{
    var source = script.target.src;
    var index = source.lastIndexOf('/');
    if(index == -1)
        throw 'Unable to read script from source path';
    var dotIndex = source.indexOf('.', index);
    if(dotIndex == -1)
        throw 'Unable to detect the filename extension in a script path';
    var module = source.substring(index + 1, dotIndex);
    var index = remainingModules.indexOf(module);
    if(index == -1)
        throw 'Unable to find module ' + module + ' in remaining modules';
    remainingModules.splice(index, 1);
    if(remainingModules.length == 0)
    {
        //All modules have been loaded, perform callback
        callback();
    }
}

//URL functions

function getURL(path)
{
    return system.baseURL + path;
}

function getBaseURL(moduleMode)
{
    var mainScript = document.getElementById('mainScript');
    if(mainScript === null)
        throw 'Unable to find the main script ID';
    var separator = '/';
    var tokens = mainScript.src.split(separator);
    if(tokens.length < 2)
        throw 'Invalid script path pattern';
    var offset = moduleMode ? -3 : -2;
    var baseURL = tokens.slice(0, offset).join(separator) + separator;
    return baseURL;
}

//Region class, used to hold information about regions in system.regions

function Region(abbreviation, description, identifier)
{
    this.abbreviation = abbreviation;
    this.description = description;
    this.identifier = identifier;
}