using System.Security.Claims;

namespace PadelApp;

public static class HttpContextExtensions
{
    public static string GetUserId(this IHttpContextAccessor httpContextAccessor)
    {
        return httpContextAccessor.HttpContext!.User.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
    }

    public static string GetUserName(this IHttpContextAccessor httpContextAccessor)
    {
        return httpContextAccessor.HttpContext!.User.Identity!.Name!;
    }
}

public static class ClaimsExtensions
{
    public static string GetUserId(this IEnumerable<Claim> claims)
    {
        return claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
    }
}