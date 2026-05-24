using Bebrakumpis.Domain.Entities;

namespace Bebrakumpis.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
