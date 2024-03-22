using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Task5.Data;
using Task5.Models;

namespace Task5.Logic;

public class BooksFileParser
{
    private struct ParsedLine(
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

    private readonly ApplicationContext _db;

    public BooksFileParser(ApplicationContext db)
    {
        this._db = db;
    }

    public void ParseFileAndSaveToDB(string filePath, bool skipFirstLine = true)
    {
        var parsedLines = ParseFile(filePath, skipFirstLine);
        SaveToDb(parsedLines);
    }
    
    private void SaveToDb(IEnumerable<ParsedLine> parsedLines)
    {
        foreach (var pl in parsedLines)
        {
            var genre = _db.Genres.FirstOrDefault(g => EF.Functions.Like(g.Name.ToLower(), $"%{pl.Genre.ToLower()}%")) ??
                        new Genre {Name = pl.Genre};
            
            var author = _db.Authors.FirstOrDefault(a => EF.Functions.Like(a.Name.ToLower(), $"%{pl.Author.ToLower()}%")) ?? 
                         new Author { Name = pl.Author };
            
            var publisher = _db.Publishers.FirstOrDefault(p =>EF.Functions.Like(p.Name.ToLower(), $"%{pl.Publisher.ToLower()}%")) ??
                            new Publisher { Name = pl.Publisher };
            
            var book = _db.Books.FirstOrDefault(
                           b => b.Author == author && b.Genre == genre && b.Pages == pl.Pages &&
                                b.Publisher == publisher &&
                                b.ReleaseDate == pl.ReleaseDate && 
                                EF.Functions.Like(b.Title.ToLower(), $"%{pl.Title.ToLower()}%")
                       ) ??
                       new Book
                       {
                           Author = author, Genre = genre, Pages = pl.Pages, Publisher = publisher,
                           ReleaseDate = pl.ReleaseDate, Title = pl.Title
                       };
            if (book.Id == default)
            {
                _db.Add(book);
            }
            _db.SaveChanges();
        }
    }
 
    private IEnumerable<ParsedLine>  ParseFile(string filePath, bool skipFirstLine)
    {
        ValidateFile(filePath);
        using (var reader = new StreamReader(filePath)) 
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            while (csv.Read())
            {
                ParsedLine record;
                try
                {    
                    record = csv.GetRecord<ParsedLine>();
                }
                catch (CsvHelperException ex)
                {
                    continue;
                }
                yield return record;
            }
        }
    }
    
    private void ValidateFile(string filePath)
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