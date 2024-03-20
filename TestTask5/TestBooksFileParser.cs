using Microsoft.EntityFrameworkCore;
using Task5.Data;
using Task5.Logic;

namespace TestTask5;

[TestClass]
public class TestBooksFileParser
{
    [TestMethod]
    public void TestMethod1()
    {
        var db = ApplicationContext.GetInMemoryContext();
        var parser = new BooksFileParser(db);
        parser.ParseFileAndSaveToDB(Path.Combine("..", "..", "..", "Files", "books.csv"));
    }
    
}