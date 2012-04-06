function showIndex(descriptionNode)
{
    var gotRegions = system.regions.length > 0;

    setTitle('Index');

    var container = diverse();
    container.id = 'indexForm';

    var description = paragraph();
    if(descriptionNode === undefined)
    {
        if(gotRegions)
            description.add('Enter the name of the summoner you want to look up:');
        else
            description.add(getErrorSpan('No regions have been configured.'));
    }
    else
        description.add(descriptionNode);

    container.add(description);
    if(system.privileged && gotRegions)
        container.add(getSearchForm(description));

    render(container);
}