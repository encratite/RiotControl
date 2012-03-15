using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		string GetRatingTable(Summoner summoner)
		{
			string[] columnTitles =
			{
				"Map",
				"Mode",
				"Games",
				"W",
				"L",
				"W - L",
				"WR",
				"Left",
				"Rating",
				"Top rating",
			};

			string caption = Markup.Caption("General Statistics");

			string firstRow = GetTableHeadRow(columnTitles);

			int rowCount = 0;
			string otherRows = "";
			foreach (var rating in summoner.Ratings)
			{
				int gamesPlayed = rating.Wins + rating.Losses;
				if (gamesPlayed == 0)
					continue;
				string row = "";
				row += Markup.TableCell(rating.Map.GetString());
				row += Markup.TableCell(rating.GameMode.GetString());
				row += Markup.TableCell(gamesPlayed.ToString());
				row += Markup.TableCell(rating.Wins.ToString());
				row += Markup.TableCell(rating.Losses.ToString());
				row += Markup.TableCell(SignumString(rating.Wins - rating.Losses));
				row += Markup.TableCell(Percentage(((double)rating.Wins) / (rating.Wins + rating.Losses)));
				row += Markup.TableCell(rating.Leaves.ToString());
				if (rating.CurrentRating == null)
					row += Markup.TableCell("?");
				else
					row += Markup.TableCell(rating.CurrentRating.ToString());
				if (rating.CurrentRating == null)
					row += Markup.TableCell("?");
				else
					row += Markup.TableCell(string.Format("{0} ({1})", rating.TopRating, SignumString(rating.CurrentRating.Value - rating.TopRating.Value)));
				otherRows += Markup.TableRow(row);
				rowCount++;
			}
			if (rowCount > 0)
			{
				string ratingTable = Markup.Table(caption + firstRow + otherRows, id: "ratingTable");
				return ratingTable;
			}
			else
				return "";
		}
	}
}
