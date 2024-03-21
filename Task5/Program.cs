

using Microsoft.EntityFrameworkCore;
using Task5.Data;
using Task5.Logic;

var path = Path.Combine("..", "..", "..", "Files", "books.csv");
var db = ApplicationContext.GetSqlServerContext();

var parser = new BooksFileParser(db);
parser.ParseFileAndSaveToDB(path);

var filter = new Filter(db){Genre = "Romance", LessThanPages = 500};
//
// var books = filter.DoFilter();
//
// foreach (var b in books)
// {
//     Console.WriteLine();
// }

