using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PadelApp.Data.Models;

public class ApplicationUser : IdentityUser<Guid>, IEntity
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Discord name is required.")]
    public string DiscordName { get; set; } = string.Empty;

    public Guid? PouleId { get; set; }
    [ForeignKey(nameof(PouleId))]
    public Poule? Poule { get; set; }
}