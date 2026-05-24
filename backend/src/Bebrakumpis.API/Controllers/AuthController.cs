using Bebrakumpis.API.Common;
using Bebrakumpis.Application.Features.Auth.Commands;
using Bebrakumpis.Application.Features.Auth.DTOs;
using Bebrakumpis.Application.Features.Auth.Queries;
using Bebrakumpis.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bebrakumpis.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(LoginCommand loginCommand, GetMeQuery getMeQuery) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await loginCommand.ExecuteAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return Unauthorized(new ProblemDetails { Detail = result.Error });

            Response.Cookies.Append("bh_auth", result.Value!, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            return Ok(new { message = "Login successful." });
        }
        catch (DomainValidationException ex)
        {
            return BadRequest(new ProblemDetails { Detail = string.Join("; ", ex.Errors) });
        }
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("bh_auth");
        return Ok(new { message = "Logged out." });
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(MeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        var result = getMeQuery.Execute(User);
        return result.ToActionResult(this);
    }
}
