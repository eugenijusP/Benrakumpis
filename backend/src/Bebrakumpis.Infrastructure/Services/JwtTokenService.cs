using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bebrakumpis.Application.Interfaces;
using Bebrakumpis.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Bebrakumpis.Infrastructure.Services;

public class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(User user)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("sub", user.Id.ToString()),
            new Claim("username", user.Username),
            new Claim("role", user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
