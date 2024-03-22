using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Task5.Data;
using Task5.Models;

namespace Task5.Logic;

public class FilterService
{
    private readonly ApplicationContext _db;

    public FilterService(ApplicationContext db)
    {
        _db = db;
    }

    public void SaveBooksToFile(string pathDirectoryToNewFile, IQueryable<Book> books)
    {
        if (!Directory.Exists(pathDirectoryToNewFile))
        {
            throw new DirectoryNotFoundException("Given directory path does not exist");
        }
        var outputFilePath = Path.Combine(pathDirectoryToNewFile, $"filtered_{DateTime.Now:yyyy-M-dd--HH-mm-ss}.csv");

        var resBooks = books.Include(b => b.Publisher)
            .Include(b => b.Genre)
            .Include(b => b.Author)
            .ToList();
        using (StreamWriter outputFile = new StreamWriter(outputFilePath))
        using (var csv = new CsvWriter(outputFile, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<ParsedBook>();
            csv.NextRecord();
            foreach (var b in resBooks)
            {
                var parsedBook = new ParsedBook()
                {
                    Title = b.Title, Pages = b.Pages, ReleaseDate = b.ReleaseDate, Author = b.Author.Name,
                    Publisher = b.Publisher.Name, Genre = b.Genre.Name
                };
                csv.WriteRecord(parsedBook);
                csv.NextRecord();
            }
        }
        
        Console.WriteLine($"File was created at {outputFilePath}");
    }


    public IQueryable<Book> DoFilter(Filter filter)
    {
        IQueryable<Book> books = _db.Books;
        
        if (!string.IsNullOrEmpty(filter.Title))
        {
            books = books.Where(b => EF.Functions.Like(b.Title.ToLower(), $"%{filter.Title.Trim().ToLower()}%"));
        }

        if (!string.IsNullOrEmpty(filter.Genre))
        {
            books = books.Where(b => EF.Functions.Like(b.Genre.Name.ToLower(), $"%{filter.Genre.Trim().ToLower()}%"));
        }

        if (!string.IsNullOrEmpty(filter.Author))
        {
            books = books.Where(b => EF.Functions.Like(b.Author.Name.ToLower(), $"%{filter.Author.Trim().ToLower()}%"));
        }

        if (!string.IsNullOrEmpty(filter.Publisher))
        {
            books = books.Where(b => EF.Functions.Like(b.Publisher.Name.ToLower(), $"%{filter.Publisher.Trim().ToLower()}%"));
        }

        if (filter.MoreThanPages.HasValue)
        {
            books = books.Where(b => b.Pages > filter.MoreThanPages);
        }

        if (filter.LessThanPages.HasValue)
        {
            books = books.Where(b => b.Pages < filter.LessThanPages);
        }

        if (filter.PublishedBefore.HasValue)
        {
            books = books.Where(b => b.ReleaseDate < filter.PublishedBefore);
        }

        if (filter.PublishedAfter.HasValue)
        {
            books = books.Where(b => b.ReleaseDate < filter.PublishedAfter);
        }

        return books;
        
    }
}

