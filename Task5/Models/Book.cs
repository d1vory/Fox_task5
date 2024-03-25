using System.ComponentModel.DataAnnotations;

namespace Task5.Models;

public class Book
{
    public Guid Id { get; set; }
    [StringLength(250)]
    public string Title { get; set; } = null!;
    public int Pages { get; set; }
    
    public Genre Genre { get; set; } = null!;
    public Author Author { get; set; } = null!;
    public Publisher Publisher { get; set; } = null!;
    
    public DateTime ReleaseDate { get; set; }

    public override string ToString()
    {
        return $"Title: {Title}, Genre: {Genre?.Name}, Author: {Author?.Name}, Publisher: {Publisher?.Name}, Pages: {Pages}, ReleaseDate: {ReleaseDate}";
    }
}