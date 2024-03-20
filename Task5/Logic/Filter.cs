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


    public Book[] DoFilter()
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            IQueryable<Book> books = db.Books;
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

            return books.ToArray();
        }
    }
}

