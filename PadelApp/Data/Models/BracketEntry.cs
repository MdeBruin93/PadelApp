namespace PadelApp.Data.Models;

public class BracketEntry : IEntity
{
    public Guid Id { get; set; }
    public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    public BracketType BracketType { get; set; }
    public string PositionCode { get; set; } = string.Empty;

    public Guid BracketId { get; set; }
    public Bracket Bracket { get; set; } = null!;

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}

public enum BracketType
{
    Upper,
    Lower
}