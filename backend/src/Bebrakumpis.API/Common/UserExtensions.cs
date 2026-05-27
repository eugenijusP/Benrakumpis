using System.Security.Claims;

namespace Bebrakumpis.API.Common;

public static class UserExtensions
{
    public static Guid? GetCurrentUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue("sub");
        return Guid.TryParse(sub, out var id) ? id : null;
    }
}
