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

//Content generation functions

main();