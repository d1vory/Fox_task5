using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Task5.Data;
using Task5.Models;

namespace Task5.Logic;

public class BooksFileParser
{
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
    
    private void SaveToDb(IEnumerable<ParsedBook> parsedLines)
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
 
    private IEnumerable<ParsedBook> ParseFile(string filePath, bool skipFirstLine)
    {
        ValidateFile(filePath);
        using (var reader = new StreamReader(filePath)) 
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            while (csv.Read())
            {
                ParsedBook record;
                try
                {    
                    record = csv.GetRecord<ParsedBook>();
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