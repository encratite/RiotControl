//Classes that are too simple to justify having their own modules

//Hash request class, used to store information about a # request string from the URL after it has been parsed

function HashRequest(name, requestArguments)
{
    this.name = name;
    this.requestArguments = requestArguments;
}

//Hash handler class, used to route hash requests to functions

function HashHandler(name, execute)
{
    this.name = name;
    this.execute = execute;
}

HashHandler.prototype.getHash = function()
{
    var requestArguments = [this.name];
    parseArguments(arguments).forEach(function(argument) {
        requestArguments.push(argument);
    });
    var path = '#' + requestArguments.join('/');
    return path;
};

//Summoner request, a common type of hash request

function SummonerRequest(region, accountId)
{
    this.region = region;
    this.accountId = accountId;
}

//Item information class, used by LeagueOfLegends.js

function Item(name, description)
{
    this.name = name;
    this.description = description;
}