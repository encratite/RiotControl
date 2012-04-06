//System initialisation

function getModules()
{
    var modules =
    [
        'ChampionNames',
        'Items',

        'Utility',

        'Classes',
        'BasicStatistics',
        'RankedStatistics',

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
    ];

    return modules;
}

function initialiseSystem(regions, privileged, revision)
{
    system = {};
    system.baseURL = getBaseURL();
    system.privileged = privileged;
    system.revision = revision;
    system.regions = [];
    for(i in regions)
    {
        var information = regions[i];
        var abbreviation = information[0];
        var description = information[1];
        var identifier = information[2];
        var region = new Region(abbreviation, description, identifier);
        system.regions[identifier] = region;
    }
}

function initialise(regions, privileged, revision, loadSingleModules)
{
    if(loadSingleModules === undefined)
        loadSingleModules = false;
    if(loadSingleModules)
    {
        initialiseSystem(regions, privileged, revision);
        loadModules(getModules(), runSystem);
    }
    else
        runSystem();
}

function revisionCheck()
{
    var oldestRevisionSupported = 221;
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
        return;
    installExtensions();
    loadIcon();
    loadStylesheet();
    hashRouting();
}

function installExtensions()
{
    String.prototype.trim = trimString;

    //used by stylesheet/favicon code
    document.head.add = addChild;

    //Used by renderWithoutTemplate
    document.body.add = addChild;
    document.body.purge = removeChildren;
}

function hashRouting()
{
    var request = getHashRequest();
    if(request == null)
    {
        system.hashDefaultHandler();
        return;
    }
    for(var i in system.hashHandlers)
    {
        var handler = system.hashHandlers[i];
        if(handler.name == request.name)
        {
            handler.execute(request.requestArguments);
            return;
        }
    }
    showError('Unknown hash path specified.');
}

//Module management

function loadModules(modules, callback)
{
    var remainingModules = modules.slice(0);
    for(var i in modules)
    {
        var module = modules[i].substring(0);
        var script = document.createElement('script');
        script.onerror = function() { alert('Unable to load module "' + module + '".'); }
        script.onload = function(script) { moduleOnLoad(script, remainingModules, callback); };
        script.src = getURL('Script/Module/' + module + '.js');
        document.body.appendChild(script);
    }
}

function moduleOnLoad(script, remainingModules, callback)
{
    console.log(script);
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

function getBaseURL()
{
    var mainScript = document.getElementById('mainScript');
    if(mainScript === null)
        throw 'Unable to find the main script ID';
    var separator = '/';
    var tokens = mainScript.src.split(separator);
    if(tokens.length < 2)
        throw 'Invalid script path pattern';
    var baseURL = tokens.slice(0, -2).join(separator) + separator;
    return baseURL;
}

//Region class, used to hold information about regions in system.regions

function Region(abbreviation, description, identifier)
{
    this.abbreviation = abbreviation;
    this.description = description;
    this.identifier = identifier;
}