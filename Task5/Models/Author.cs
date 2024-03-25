using System.ComponentModel.DataAnnotations;

namespace Task5.Models;

public class Author
{
    public Guid Id { get; set; }
    [StringLength(250)]
    public string Name { get; set; } = null!;
}