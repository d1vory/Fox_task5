using Task5.Data;
using Task5.Logic;
using Task5.Models;

namespace TestTask5;

[TestClass]
public class TestFilter
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
            (new Filter(db){Title = "To Kill a Mockingbird"}, 1),
            (new Filter(db){Title = "nonexisting"}, 0),
            
            (new Filter(db){Genre = "Fiction"}, 3),
            (new Filter(db){Genre = "nonexisting"}, 0),
            
            (new Filter(db){Author = "Harper Lee"}, 2),
            (new Filter(db){Author = "George Orwell"}, 1),
            (new Filter(db){Author = "nonexisting"}, 0),
            
            (new Filter(db){Publisher = "HarperCollins"}, 3),
            (new Filter(db){Publisher = "Penguin Classics"}, 1),
            (new Filter(db){Publisher = "nonexisting"}, 0),
            
            (new Filter(db){MoreThanPages = 300}, 2),
            (new Filter(db){MoreThanPages = 5}, 4),
            (new Filter(db){MoreThanPages = 500}, 0),
            (new Filter(db){MoreThanPages = 499}, 1),
            
            (new Filter(db){LessThanPages = 300}, 2),
            (new Filter(db){LessThanPages = 5}, 0),
            (new Filter(db){LessThanPages = 501}, 4),
            (new Filter(db){LessThanPages = 499}, 3),
            
            (new Filter(db){PublishedBefore = new DateTime(1939, 9, 1)}, 1),
            (new Filter(db){PublishedBefore = new DateTime(2020, 9, 1)}, 4),
            (new Filter(db){PublishedBefore = new DateTime(1970, 9, 1)}, 3),
            
            (new Filter(db){PublishedAfter = new DateTime(1939, 9, 1)}, 1),
            (new Filter(db){PublishedAfter = new DateTime(2020, 9, 1)}, 4),
            (new Filter(db){PublishedAfter = new DateTime(1970, 9, 1)}, 3),
            
            (new Filter(db){Author = "George Orwell", Genre = "Fiction"}, 1),
            (new Filter(db){Author = "Harper Lee", Genre = "Fiction"}, 2),
            (new Filter(db){Author = "Harper Lee", Genre = "Fiction", MoreThanPages = 50}, 2),
            (new Filter(db){Author = "Harper Lee", Genre = "Fiction", MoreThanPages = 150}, 1),
            (new Filter(db){Author = "Harper Lee", Genre = "Fiction", Title = "To Kill a Mockingbird",}, 1),
            (new Filter(db){Author = "Harper Lee", Genre = "Fiction", PublishedBefore = new DateTime(1970, 9, 1)}, 1),
            (new Filter(db){Author = "Harper Lee", Genre = "Fiction", PublishedAfter = new DateTime(1970, 9, 1)}, 1),
        };


        foreach (var (filter, expectedCount) in filterAndExpectedCount)
        {
            var result = filter.DoFilter();
            try
            {
                Assert.AreEqual(expectedCount, result.Length);
            }
            catch (AssertFailedException)
            {
                Console.WriteLine(filter);
                throw;
            }
        }
        
    }

}