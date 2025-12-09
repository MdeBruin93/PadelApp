using System.ComponentModel.DataAnnotations;

namespace PadelApp.Data.EditModels;

public class EditMatch
{
    public Guid Id { get; set; }
    public string PouleName { get; set; } = string.Empty;

    // Add scores for TeamA and TeamB
    [Range(0, int.MaxValue, ErrorMessage = "Score must be non-negative.")]
    public int ScoreTeamA { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Score must be non-negative.")]
    public int ScoreTeamB { get; set; }
}