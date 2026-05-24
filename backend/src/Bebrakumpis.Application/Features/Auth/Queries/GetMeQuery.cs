using System.Security.Claims;
using Bebrakumpis.Application.Common.Result;
using Bebrakumpis.Application.Features.Auth.DTOs;

namespace Bebrakumpis.Application.Features.Auth.Queries;

public class GetMeQuery
{
    public Result<MeResponse> Execute(ClaimsPrincipal principal)
    {
        var sub = principal.FindFirst("sub")?.Value;
        if (sub is null || !Guid.TryParse(sub, out var userId))
            return Result<MeResponse>.Unauthorized("Not authenticated.");

        var username = principal.FindFirst("username")?.Value ?? string.Empty;
        var role = principal.FindFirst("role")?.Value ?? string.Empty;

        return Result<MeResponse>.Success(new MeResponse
        {
            Id = userId,
            Username = username,
            Role = role
        });
    }
}
