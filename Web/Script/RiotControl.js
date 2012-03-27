function getURL(path)
{
    return getSourceURL() + path;
}

function isUndefined(input)
{
    return typeof input == 'undefined';
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
    this.baseURL = getBaseURL();
}

function main()
{
    try
    {
        system = new System();
    }
    catch(exception)
    {
        alert('Initialisation error: ' + exception);
    }
}

main();