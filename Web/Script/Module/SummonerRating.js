function getRatingTable(statistics)
{
    var ratings = statistics.Ratings;

    var columnTitles =
        [
            'Map',
            'Mode',
            'Games',
            'W',
            'L',
            'W - L',
            'WR',
            'Left',
            'Rating',
            'Top rating',
        ];

    var output = table();
    output.id = 'ratingTable';

    output.add(caption('General Statistics'));

    output.add(getTableHeadRow(columnTitles));

    var rowCount = 0;
    ratings.forEach(function(rating) {
        var gamesPlayed = rating.Wins + rating.Losses;
        if (gamesPlayed == 0)
            return;
        var mapString;
        if(rating.Map == 1 && rating.GameMode == 2)
        {
            //This is not actually Unranked Summoner's Rift data but collective data for both Summoner's Rift and Twisted Treeline
            mapString = "Summoner's Rift/Twisted Treeline";
        }
        else
            mapString = getMapString(rating.Map);
        var fields =
            [
                mapString,
                getGameModeString(rating.GameMode),
                gamesPlayed,
                rating.Wins,
                rating.Losses,
                signum(rating.Wins - rating.Losses),
                percentage(rating.Wins / (rating.Wins + rating.Losses)),
                rating.Leaves,
                getCurrentRating(rating),
                getTopRating(rating),
            ];
        var row = tableRow();
        fields.forEach(function(field) {
            row.add(tableCell(field));
        });
        output.add(row);
        rowCount++;
    });
    if(rowCount > 0)
        return output;
    else
        return '';
}