using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Task5.Data;
using Task5.Logic;

namespace Task5;
class Program
{
    public static void Main(string[] args)
    {
        var db = ApplicationContext.GetSqlServerContext();
        var parser = new BooksFileParser(db);
        parser.ParseFileAndSaveToDB(
            "C:\\Users\\Olexandr\\MyProjects\\CSHARP\\foxminded\\Task5\\Task5\\Files\\books.csv");
        
        // Console.WriteLine(
        //     "This is a program which works with books\n" + 
        //     "To start, choose a mode to work: \n" +
        //     "'1' Upload new books to database, using existing csv file\n" +
        //     "'2' Filter existing books\n" +
        //     "'3' to exit program");
        // var shouldExit = false;
        // do
        // {
        //     Console.WriteLine("Enter mode: ");
        //     var userInput = Console.ReadLine();
        //     int.TryParse(userInput, out int mode);
        //     switch (mode)
        //     {
        //         case 1:
        //             UploadNewBooks();
        //             break;
        //         case 2:
        //             FilterExistingBooks();
        //             break;
        //         case 3:
        //             shouldExit = true;
        //             break;
        //         default:
        //             Console.WriteLine("Given mode is not valid");
        //             break;
        //     }
        // } while (!shouldExit);
    }

    private static void UploadNewBooks()
    {
        Console.WriteLine("_____Upload new books_____");
        Console.WriteLine("To exit, type 'exit'");
        var db = ApplicationContext.GetSqlServerContext();
        var parser = new BooksFileParser(db);
        
        while (true)
        {
            try
            {
                Console.WriteLine("Enter a path to csv file: ");
                string value = Console.ReadLine();
                if (value == "exit")
                {
                    break;
                }

                if (value != null) parser.ParseFileAndSaveToDB(value);
                Console.WriteLine("File was successfully parsed and saved to DB");
                Console.WriteLine("To exit, type 'exit'");

            }
            catch (Exception ex) when (ex is ApplicationException or FileNotFoundException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

        }
    }

    private static void FilterExistingBooks()
    {
        Console.WriteLine("_____Filter existing books in the database_____");
        Console.WriteLine("To exit, type 'exit'");
        var db = ApplicationContext.GetSqlServerContext();
        
        while (true)
        {

            Console.WriteLine("Enter filter criteria. To skip a criteria, press enter");

            var title = GetStringFilterCriteria("Title");
            var genre = GetStringFilterCriteria("Genre");
            var author = GetStringFilterCriteria("Author");
            var publisher = GetStringFilterCriteria("Publisher");
            var moreThanPages = GetIntFilterCriteria("MoreThanPages");
            var lessThanPages = GetIntFilterCriteria("LessThanPages");
            var publishedBefore = GetDateTimeFilterCriteria("PublishedBefore");
            var publishedAfter = GetDateTimeFilterCriteria("PublishedAfter");

            var filterService = new FilterService(db);
            var filter = new Filter(){
                Title = title, Genre = genre, Author = author, Publisher = publisher, MoreThanPages = moreThanPages,
                LessThanPages = lessThanPages, PublishedBefore = publishedBefore, PublishedAfter = publishedAfter
            };
            
            Console.WriteLine("Enter a path to directory where file will be stored");
            string pathDirectoryToNewFile = Console.ReadLine();
            while (!Directory.Exists(pathDirectoryToNewFile))
            {
                Console.WriteLine("Given path does not exist!, try again.");
                Console.WriteLine("Enter a path to directory where file will be stored");
                pathDirectoryToNewFile = Console.ReadLine();
            }

            var books = filterService.DoFilter(filter)
                .Include(b => b.Publisher)
                .Include(b => b.Genre)
                .Include(b => b.Author);
            Console.WriteLine("=========================================");
            foreach (var book in books.ToList())
            {
                Console.WriteLine(book);
            }
            Console.WriteLine("=========================================");
            
            filterService.SaveBooksToFile(pathDirectoryToNewFile, books);

            
            Console.WriteLine("Continue? To exit, type 'exit'");
            string value = Console.ReadLine();
            if (value == "exit")
            {
                break;
            }
        }
        
    }
    
    private static int? GetIntFilterCriteria(string criteriaName)
    {
        while (true)
        {
            Console.WriteLine($"{criteriaName}: ");
            string value = Console.ReadLine();

            if (value.IsNullOrEmpty())
            {
                Console.WriteLine($"{criteriaName} skipped ");
                return null;
            }
            
            var isValid = int.TryParse(value, out var res);
            if (isValid)
            {
                return res;
            }
            Console.WriteLine("Given value is not valid");
        }
    }
    
    private static string? GetStringFilterCriteria(string criteriaName)
    {
        Console.WriteLine($"{criteriaName}: ");
        string value = Console.ReadLine();

        if (value.IsNullOrEmpty())
        {
            Console.WriteLine($"{criteriaName} skipped ");
            return null;
        }

        return value;
    }
    
    private static DateTime? GetDateTimeFilterCriteria(string criteriaName)
    {
        while (true)
        {
            Console.WriteLine($"{criteriaName}: ");
            string value = Console.ReadLine();

            if (value.IsNullOrEmpty())
            {
                Console.WriteLine($"{criteriaName} skipped ");
                return null;
            }
            
            var isValid = DateTime.TryParse(value, out var res);
            if (isValid)
            {
                return res;
            }
            Console.WriteLine("Given value is not valid");
        }
    }
 
    
}
