using PadelApp.Data.Models;

namespace PadelApp.Services;

public class PlayerStatsEntry
{
    public ApplicationUser Player { get; set; } = default!;
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int MatchesLost { get; set; }
    public int Draws { get; set; }
    public int PointsScored { get; set; }
    public int PointsLost { get; set; }
    public int TotalPoints { get; set; }
    public int Position { get; set; }
}