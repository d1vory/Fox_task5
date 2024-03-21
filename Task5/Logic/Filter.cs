using Microsoft.EntityFrameworkCore;
using Task5.Data;
using Task5.Models;

namespace Task5.Logic;

public class Filter
{
    public string? Title {get; set; }
    public string? Genre {get; set; }
    public string? Author {get; set; }
    public string? Publisher {get; set; }
    public int? MoreThanPages {get;set;}
    public int? LessThanPages {get;set;}
    public DateTime? PublishedBefore {get;set;}
    public DateTime? PublishedAfter{get;set;}
    
    private readonly ApplicationContext _db;

    public Filter(ApplicationContext db)
    {
        _db = db;
    }

    public void SaveBooksToFile(string pathDirectoryToNewFile, IQueryable<Book> books)
    {
        if (!Directory.Exists(pathDirectoryToNewFile))
        {
            throw new DirectoryNotFoundException("Given directory path does not exist");
        }
        var outputFilePath = Path.Combine(pathDirectoryToNewFile, $"filtered_{DateTime.Now}.txt");

        books.Include(b => b.Publisher)
            .Include(b => b.Genre)
            .Include(b => b.Author);
        using (StreamWriter outputFile = new StreamWriter(outputFilePath))
        {
            foreach (var book in books.ToList())
                outputFile.WriteLine(book);
        }
        Console.WriteLine($"File was created at {outputFilePath}");
    }


    public IQueryable<Book> DoFilter()
    {
        IQueryable<Book> books = _db.Books;
        if (!string.IsNullOrEmpty(Title))
        {
            books = books.Where(b => b.Title == Title);
        }
        if (!string.IsNullOrEmpty(Genre))
        {
            books = books.Include(b=>b.Genre).Where(b => b.Genre.Name == Genre);
        }
        if (!string.IsNullOrEmpty(Author))
        {
            books = books.Include(b=>b.Author).Where(b => b.Author.Name == Author);
        }
        if (!string.IsNullOrEmpty(Publisher))
        {
            books = books.Include(b=>b.Publisher).Where(b => b.Publisher.Name == Publisher);
        }
        if (MoreThanPages.HasValue)
        {
            books = books.Where(b => b.Pages > MoreThanPages);
        }
        if (LessThanPages.HasValue)
        {
            books = books.Where(b => b.Pages < LessThanPages);
        }
        if (PublishedBefore.HasValue)
        {
            books = books.Where(b => b.ReleaseDate < PublishedBefore);
        }
        if (PublishedAfter.HasValue)
        {
            books = books.Where(b => b.ReleaseDate < PublishedAfter);
        }

        return books;
        
    }

    public override string ToString()
    {
        return
            $"Title: {Title}, Genre: {Genre}, Author: {Author}, Publisher: {Publisher}, MoreThanPages: {MoreThanPages}," +
            $" LessThanPages: {LessThanPages}, PublishedBefore: {PublishedBefore}, PublishedAfter: {PublishedAfter}";
    }
}

