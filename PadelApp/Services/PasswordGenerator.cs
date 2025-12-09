namespace PadelApp.Services;

public static class PasswordGenerator
{
    public static string GeneratePassword()
    {
        return Guid.NewGuid().ToString("N");
    }
}