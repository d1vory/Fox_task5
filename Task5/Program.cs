

using Task5.Logic;

var path = Path.Combine("..", "..", "..", "Files", "books.csv");

BooksFileParser.ParseFileAndSaveToDB(path);