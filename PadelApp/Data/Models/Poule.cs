using System.ComponentModel.DataAnnotations;

namespace PadelApp.Data.Models;

public class Poule : IEntity
{
    public Guid Id { get; set; }
    public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    // Add code property (A, B, C, ...)
    [Required]
    [StringLength(2, ErrorMessage = "Code must be two letters.")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    public string Name { get; set; } = string.Empty;

    public List<ApplicationUser> Players { get; set; } = new();
}