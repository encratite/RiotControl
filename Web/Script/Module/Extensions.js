//Extensions to standard structures

function installExtensions()
{
    //used by stylesheet/favicon code
    document.head.add = addChild;

    //Used by renderWithoutTemplate
    document.body.add = addChild;
    document.body.purge = removeChildren;
}

Array.prototype.isArray = function()
{
    return true;
}

String.prototype.trim = function()
{
    return this.replace(/^\s+|\s+$/g, '');
}