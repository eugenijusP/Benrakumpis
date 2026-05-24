using System.Security.Claims;
using Bebrakumpis.Application.Common.CQRS;
using Bebrakumpis.Application.Common.Result;
using Bebrakumpis.Application.Features.Auth.DTOs;

namespace Bebrakumpis.Application.Features.Auth.Queries;

public record GetMeQuery(ClaimsPrincipal Principal) : IRequest<Result<MeResponse>>;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, Result<MeResponse>>
{
    public Task<Result<MeResponse>> HandleAsync(GetMeQuery query, CancellationToken ct)
    {
        var sub = query.Principal.FindFirst("sub")?.Value;
        if (sub is null || !Guid.TryParse(sub, out var userId))
            return Task.FromResult(Result<MeResponse>.Unauthorized("Not authenticated."));

        var username = query.Principal.FindFirst("username")?.Value ?? string.Empty;
        var role = query.Principal.FindFirst("role")?.Value ?? string.Empty;

        return Task.FromResult(Result<MeResponse>.Success(new MeResponse
        {
            Id = userId,
            Username = username,
            Role = role
        }));
    }
}
