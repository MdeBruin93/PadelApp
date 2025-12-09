using System.ComponentModel.DataAnnotations;

namespace PadelApp.Data.Models;

public class TournamentSettings : IEntity
{
    [Key]
    public Guid Id { get; set; }
    public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    public bool BracketsReleased { get; set; }
    public bool PoulesReleased { get; set; }
}