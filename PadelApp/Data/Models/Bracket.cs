namespace PadelApp.Data.Models;

public class Bracket : IEntity
{
    public Guid Id { get; set; }
    public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    public BracketType BracketType { get; set; }
    public List<BracketEntry> Entries { get; set; } = null!;
    public List<BracketMatch> Matches { get; set; } = null!;
}