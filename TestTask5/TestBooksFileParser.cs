using Microsoft.EntityFrameworkCore;
using Task5.Data;
using Task5.Logic;

namespace TestTask5;

[TestClass]
public class TestBooksFileParser
{
    private readonly string _booksPath1 = Path.Combine("..", "..", "..", "Files", "books_test.csv");
    private readonly string _booksPath2 = Path.Combine("..", "..", "..", "Files", "books_test2.csv");
    
    [TestMethod]
    public void TestParsing()
    {
        var db = ApplicationContext.GetInMemoryContext();
        var parser = new BooksFileParser(db);
        parser.ParseFileAndSaveToDB(_booksPath1);

        var expectedBookTitles = new[]
        {
            "To Kill a Mockingbird",
            "1984",
            "The Great Gatsby",
            "Pride and Prejudice",
            "Harry Potter and the Sorcerer's Stone",
            "The Catcher in the Rye",
            "To the Lighthouse"
        };
        var dbBookTitles = db.Books.Select(x => x.Title).ToArray();
        Assert.IsTrue(expectedBookTitles.SequenceEqual(dbBookTitles));


        var expectedGenres = new[]
        {
            "Fiction",
            "Romance",
            "Fantasy",
            "Coming of Age",
            "Modernist"
        };
        var dbGenres = db.Genres.Select(x => x.Name).ToArray();
        Assert.IsTrue(expectedGenres.SequenceEqual(dbGenres));

        var expectedAuthors = new[]
        {
            "Harper Lee",
            "George Orwell",
            "Jane Austen",
            "J.K. Rowling",
            "J.D. Salinger",
            "Virginia Woolf"
        };
        var dbAuthors  = db.Authors .Select(x => x.Name).ToArray();
        Assert.IsTrue(expectedAuthors.SequenceEqual(dbAuthors));

        var expectedPublishers = new[]
        {
            "HarperCollins",
            "Signet Classics",
            "Scribner",
            "Penguin Classics",
            "Scholastic"
        };
        var dbPublishers  = db.Publishers.Select(x => x.Name).ToArray();
        Assert.IsTrue(expectedPublishers.SequenceEqual(dbPublishers));

    }

    [TestMethod]
    public void TestRepeatedParsing()
    {
        var db = ApplicationContext.GetInMemoryContext();
        var parser = new BooksFileParser(db);
        parser.ParseFileAndSaveToDB(_booksPath1);
        
        Assert.AreEqual(7, db.Books.Count());
        Assert.AreEqual(5, db.Genres.Count());
        Assert.AreEqual(6, db.Authors.Count());
        Assert.AreEqual(5, db.Publishers.Count());
        
        parser.ParseFileAndSaveToDB(_booksPath1);
        parser.ParseFileAndSaveToDB(_booksPath1);
        
        Assert.AreEqual(7, db.Books.Count());
        Assert.AreEqual(5, db.Genres.Count());
        Assert.AreEqual(6, db.Authors.Count());
        Assert.AreEqual(5, db.Publishers.Count());
        
        parser.ParseFileAndSaveToDB(_booksPath2);
        
        Assert.AreEqual(8, db.Books.Count());
        Assert.AreEqual(6, db.Genres.Count());
        Assert.AreEqual(7, db.Authors.Count());
        Assert.AreEqual(6, db.Publishers.Count());
        
    }
    
}