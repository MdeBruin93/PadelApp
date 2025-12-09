namespace PadelApp.Data.Models;

public class DataVersion : IEntity
{
    public Guid Id { get; set; }
    public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    public Version Version { get; set; }
}

public enum Version
{
    One,
    Two,
    Three
}