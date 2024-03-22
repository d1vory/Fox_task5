namespace Task5.Logic;

public class ParsedBook(
    string title,
    string pages,
    string genre,
    string releaseDate,
    string author,
    string publisher)
{
    public string Title { get; set; } = title.Trim();
    public int Pages { get; set; } = int.Parse(pages);
    public string Genre { get; set; } = genre.Trim();
    public DateTime ReleaseDate { get; set; } = DateTime.Parse(releaseDate);
    public string Author { get; set; } = author.Trim();
    public string Publisher { get; set; } = publisher.Trim();
    public const int PropertyCount = 6;
}