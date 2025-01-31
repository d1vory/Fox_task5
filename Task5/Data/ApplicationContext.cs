using Microsoft.EntityFrameworkCore;
using Task5.Models;

namespace Task5.Data;

public class ApplicationContext: DbContext
{
    public DbSet<Author> Authors  { get; set; } = null!;
    public DbSet<Book> Books   { get; set; } = null!;
    public DbSet<Genre> Genres   { get; set; } = null!;
    public DbSet<Publisher> Publishers  { get; set; } = null!;
    
    public ApplicationContext() { }
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    public static ApplicationContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;
        return new ApplicationContext(options);
    }

    public static ApplicationContext GetSqlServerContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlServer(@"data source=o-dubchak-pc2\SQLEXPRESS;initial catalog=books;trusted_connection=true;TrustServerCertificate=True;")
            .Options;
        return new ApplicationContext(options);
    }
    
}