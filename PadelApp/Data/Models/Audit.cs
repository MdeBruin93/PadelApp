namespace PadelApp.Data.Models;

public class Audit : IEntity
{
    public Guid Id { get; set; }
    public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The type of action performed (e.g., "UpdatePouleMatchScore", "UpdateBracketMatchScore", "ReleaseBrackets")
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// The username of the person who performed the action
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Details about the action in a human-readable format
    /// </summary>
    public string Details { get; set; } = string.Empty;

    /// <summary>
    /// Optional JSON data with structured information about the action
    /// </summary>
    public string? ActionData { get; set; }

    /// <summary>
    /// When the action was performed
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}