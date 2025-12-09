namespace PadelApp.Data.Models;

public interface IEntity
{
    Guid Id { get; set; }
    string? ConcurrencyStamp { get; set; }
}