using System.ComponentModel.DataAnnotations;

namespace PadelApp.Data.Models;

public class Match : IEntity
{
    public Guid Id { get; set; }

    public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    // Add scores for TeamA and TeamB
    [Range(0, int.MaxValue, ErrorMessage = "Score must be non-negative.")]
    public int ScoreTeamA { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Score must be non-negative.")]
    public int ScoreTeamB { get; set; }

    // Add a property to indicate if the match is finished
    public bool IsFinished { get; set; }

    [Range(2, 6)]
    public int Court { get; set; }

    [Required(ErrorMessage = "Start time is required.")]
    public DateTime StartTime { get; set; }

    public Guid PouleId { get; set; }
    public Poule Poule { get; set; } = null!;

    public List<ApplicationUser> Players { get; set; } = new();
    public List<ApplicationUser> TeamA { get; set; } = new();
    public List<ApplicationUser> TeamB { get; set; } = new();
}