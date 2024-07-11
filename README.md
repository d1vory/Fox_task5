# Books Console Application with Entity Framework Core

## Description

This project is a console application developed using .NET 7 and Entity Framework Core with a code-first approach. The application interacts with a SQL Server database to manage book information. It includes functionalities to add books from a file and search for books using various filters.

## Prerequisites

- .NET 7
- SQL Server DB
- Entity Framework Core


## Features

### Add Books from File

1. **File Input:**
    - User provides a file path via console input.
    - The program parses the file and stores the data in the database.
    - Duplicate entries are not created if the program is run with the same file twice.

2. **Search Books:**
    - Implement book search functionality using filters.
    - Introduce a `Filter` class with optional fields to specify filtering criteria. These filter settings should be specified in an external JSON file `filter.json`.

      ```csharp
      public class Filter
      {
          public string? Title { get; set; }
          public string? Genre { get; set; }
          public string? Author { get; set; }
          public string? Publisher { get; set; }
          public int? MoreThanPages { get; set; }
          public int? LessThanPages { get; set; }
          public DateTime? PublishedBefore { get; set; }
          public DateTime? PublishedAfter { get; set; }
      }
      ```

    - Display the count of books matching the query and list the titles of those books in the console.
    - Save the output to a uniquely named file (using the date/time of saving) containing a list of books matching the query in the same format as the original input file.
