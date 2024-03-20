

using Task5.Logic;

var path = Path.Combine("..", "..", "..", "Files", "books.csv");

//BooksFileParser.ParseFileAndSaveToDB(path);

var filter = new Filter(){Genre = "Romance", LessThanPages = 500};

var books = filter.DoFilter();

foreach (var b in books)
{
    Console.WriteLine();
}