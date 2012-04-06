//Classes that are too simple to justify having their own modules

//Contains information about the HTTP path requested

function Request(name, requestArguments)
{
    this.name = name;
    this.requestArguments = requestArguments;
}

//Request handler class, used to route requests to functions

function RequestHandler(name, execute)
{
    this.name = name;
    this.execute = execute;
}

RequestHandler.prototype.getPath = function()
{
    var requestArguments = [this.name];
    parseArguments(arguments).forEach(function(argument) {
        requestArguments.push(argument);
    });
    var separator = '/';
    var path = separator + requestArguments.join(separator);
    return path;
};

RequestHandler.prototype.open = function()
{
    var path = RequestHandler.prototype.getPath.apply(this, parseArguments(arguments));
    location.href = path;
}

//Summoner request, a common type of request

function SummonerRequest(region, accountId)
{
    this.region = region;
    this.accountId = accountId;
}