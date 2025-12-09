using PadelApp.Services;

namespace PadelApp.Components.Pages;

public class BracketEntryViewModel
{
    public string PositionCode { get; set; } = string.Empty;
    public Guid UserId { get; set; } = Guid.Empty;
    public string User { get; set; } = string.Empty;
    public PlayerStatsEntry Stats { get; set; } = null!;
}