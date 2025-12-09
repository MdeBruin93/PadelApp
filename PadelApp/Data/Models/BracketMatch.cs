using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PadelApp.Data.Models;

public class BracketMatch : IEntity
{
    public Guid Id { get; set; }
    public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    public int ScoreTeamA { get; set; }
    public int ScoreTeamB { get; set; }
    public bool IsFinished { get; set; }

    [Range(2, 6)]
    public int Court { get; set; }
    public int BracketRoundNumber { get; set; }
    public BracketType BracketType { get; set; }

    [Required(ErrorMessage = "Start time is required.")]
    public DateTime StartTime { get; set; }

    public Guid BracketId { get; set; }
    [ForeignKey(nameof(BracketId))]
    public Bracket Bracket { get; set; } = null!;

    public List<ApplicationUser> Players { get; set; } = null!;
    public List<ApplicationUser> TeamA { get; set; } = null!;
    public List<ApplicationUser> TeamB { get; set; } = null!;
}