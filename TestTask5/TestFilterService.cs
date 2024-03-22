using Task5.Data;
using Task5.Logic;
using Task5.Models;

namespace TestTask5;

[TestClass]
public class TestFilterService
{
    public ApplicationContext db;
    
    [TestInitialize]
    public void TestInitialize()
    {
        db = ApplicationContext.GetInMemoryContext();

        var genre1 = new Genre() { Name = "Fiction" };
        var genre2 = new Genre() { Name = "Romance" };

        var author1 = new Author() { Name = "Harper Lee" };
        var author2 = new Author() { Name = "George Orwell" };
        var author3 = new Author() { Name = "Jane Austen" };

        var publisher1 = new Publisher() { Name = "HarperCollins" };
        var publisher2 = new Publisher() { Name = "Penguin Classics" };

        var books = new[]
        {
            new Book()
            {
                Title = "To Kill a Mockingbird", Pages = 200, ReleaseDate = new DateTime(1960, 7, 11),
                Genre = genre1, Author = author1, Publisher = publisher1
            },
            new Book()
            {
                Title = "1984", Pages = 328, ReleaseDate = new DateTime(1949, 6, 8),
                Genre = genre1, Author = author2, Publisher = publisher1
            },
            new Book()
            {
                Title = "Pride and Prejudice", Pages = 500, ReleaseDate = new DateTime(1813, 1, 28),
                Genre = genre2, Author = author3, Publisher = publisher2
            },
            new Book()
            {
                Title = "The Catcher in the Rye", Pages = 100, ReleaseDate = new DateTime(2001, 1, 1),
                Genre = genre1, Author = author1, Publisher = publisher1
            },
        };
        
        db.AddRange(books);
        db.SaveChanges();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    [TestMethod]
    public void TestFiltering()
    {
        var filterAndExpectedCount = new (Filter filter, int expectedCount)[]
        {
            (new Filter(){Title = "To Kill a Mockingbird"}, 1),
            (new Filter(){Title = "nonexisting"}, 0),
            
            (new Filter(){Genre = "Fiction"}, 3),
            (new Filter(){Genre = "nonexisting"}, 0),
            
            (new Filter(){Author = "Harper Lee"}, 2),
            (new Filter(){Author = "George Orwell"}, 1),
            (new Filter(){Author = "nonexisting"}, 0),
            
            (new Filter(){Publisher = "HarperCollins"}, 3),
            (new Filter(){Publisher = "Penguin Classics"}, 1),
            (new Filter(){Publisher = "nonexisting"}, 0),
            
            (new Filter(){MoreThanPages = 300}, 2),
            (new Filter(){MoreThanPages = 5}, 4),
            (new Filter(){MoreThanPages = 500}, 0),
            (new Filter(){MoreThanPages = 499}, 1),
            
            (new Filter(){LessThanPages = 300}, 2),
            (new Filter(){LessThanPages = 5}, 0),
            (new Filter(){LessThanPages = 501}, 4),
            (new Filter(){LessThanPages = 499}, 3),
            
            (new Filter(){PublishedBefore = new DateTime(1939, 9, 1)}, 1),
            (new Filter(){PublishedBefore = new DateTime(2020, 9, 1)}, 4),
            (new Filter(){PublishedBefore = new DateTime(1970, 9, 1)}, 3),
            
            (new Filter(){PublishedAfter = new DateTime(1939, 9, 1)}, 1),
            (new Filter(){PublishedAfter = new DateTime(2020, 9, 1)}, 4),
            (new Filter(){PublishedAfter = new DateTime(1970, 9, 1)}, 3),
            
            (new Filter(){Author = "George Orwell", Genre = "Fiction"}, 1),
            (new Filter(){Author = "Harper Lee", Genre = "Fiction"}, 2),
            (new Filter(){Author = "Harper Lee", Genre = "Fiction", MoreThanPages = 50}, 2),
            (new Filter(){Author = "Harper Lee", Genre = "Fiction", MoreThanPages = 150}, 1),
            (new Filter(){Author = "Harper Lee", Genre = "Fiction", Title = "To Kill a Mockingbird",}, 1),
            (new Filter(){Author = "Harper Lee", Genre = "Fiction", PublishedBefore = new DateTime(1970, 9, 1)}, 1),
            (new Filter(){Author = "Harper Lee", Genre = "Fiction", PublishedAfter = new DateTime(1970, 9, 1)}, 1),
        };

        var filterService = new FilterService(db);
        foreach (var (filter, expectedCount) in filterAndExpectedCount)
        {
            var result = filterService.DoFilter(filter);
            try
            {
                Assert.AreEqual(expectedCount, result.Count());
            }
            catch (AssertFailedException)
            {
                Console.WriteLine(filter);
                throw;
            }
        }
        
    }

}