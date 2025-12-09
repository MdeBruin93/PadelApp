namespace PadelApp.Components.Pages.Users;

public class UserViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DiscordName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}