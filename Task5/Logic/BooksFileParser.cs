using Task5.Data;
using Task5.Models;

namespace Task5.Logic;

public static class BooksFileParser
{
    private struct ParsedLine(
        string title,
        string pages,
        string genre,
        string releaseDate,
        string author,
        string publisher)
    {
        public string Title { get; set; } = title;
        public int Pages { get; set; } = int.Parse(pages);
        public string Genre { get; set; } = genre;
        public DateTime ReleaseDate { get; set; } = DateTime.Parse(releaseDate);
        public string Author { get; set; } = author;
        public string Publisher { get; set; } = publisher;
        public const int PropertyCount = 6;
    }

    public static void ParseFileAndSaveToDB(string filePath, bool skipFirstLine = true)
    {
        var parsedLines = ParseFile(filePath, skipFirstLine);
        SaveToDb(parsedLines);
    }
    
    private static void SaveToDb(ParsedLine[] parsedLines)
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            foreach (var pl in parsedLines)
            {
                var genre = db.Genres.FirstOrDefault(g => g.Name == pl.Genre) ??
                            new Genre {Name = pl.Genre};
                
                var author = db.Authors.FirstOrDefault(a => a.Name == pl.Author) ?? 
                             new Author { Name = pl.Author };
                
                var publisher = db.Publishers.FirstOrDefault(p => p.Name == pl.Publisher) ??
                                new Publisher { Name = pl.Publisher };
                
                var book = db.Books.FirstOrDefault(
                               b => b.Author == author && b.Genre == genre && b.Pages == pl.Pages &&
                                    b.Publisher == publisher &&
                                    b.ReleaseDate == pl.ReleaseDate && b.Title == pl.Title
                           ) ??
                           new Book
                           {
                               Author = author, Genre = genre, Pages = pl.Pages, Publisher = publisher,
                               ReleaseDate = pl.ReleaseDate, Title = pl.Title
                           };
                if (book.Id == default)
                {
                    db.Add(book);
                }
                db.SaveChanges();
            }
        }
    }
 
    private static ParsedLine[] ParseFile(string filePath, bool skipFirstLine)
    {
        ValidateFile(filePath);

        var parsedLines = new List<ParsedLine>();
        
        foreach (var line in File.ReadLines(filePath))
        {
            if (skipFirstLine)
            {
                skipFirstLine = false;
                continue;
            }
            var isLineValid = TryParseLine(line, out var parsedLine);
            if (isLineValid)
            {
                parsedLines.Add(parsedLine);
            }
            
        }

        return parsedLines.ToArray();
    }
    

    private static bool TryParseLine(string line, out ParsedLine parsedLine)
    {
        var elements = line.Split(',');
        if (elements.Length != ParsedLine.PropertyCount)
        {
            parsedLine = default;
            return false;
        }

        try
        {
             parsedLine = new ParsedLine(elements[0], elements[1],elements[2],elements[3],elements[4],elements[5]);
        }
        catch (FormatException e)
        {
            parsedLine = default;
            return false;
        }
        
        return true;
    }

    private static void ValidateFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The file does not exist by this path");
        }
        if (!filePath.EndsWith(".csv"))
        {
            throw new ApplicationException("File should have '.csv' extension");
        }
        
    }
}