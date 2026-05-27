namespace Bebrakumpis.Application.Features.Auth.DTOs;

public class MeResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
