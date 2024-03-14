using Microsoft.EntityFrameworkCore;
using Task5.Models;

namespace Task5.Data;

public class ApplicationContext: DbContext
{
    public DbSet<Author> Authors  { get; set; } = null!;
    public DbSet<Book> Books   { get; set; } = null!;
    public DbSet<Genre> Genres   { get; set; } = null!;
    public DbSet<Publisher> Publishers  { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"data source=o-dubchak-pc2\SQLEXPRESS;initial catalog=books;trusted_connection=true;TrustServerCertificate=True;");
    }
}