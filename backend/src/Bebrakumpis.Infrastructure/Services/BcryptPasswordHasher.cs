using Bebrakumpis.Application.Interfaces;

namespace Bebrakumpis.Infrastructure.Services;

public class BcryptPasswordHasher : IPasswordHasher
{
    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);

    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);
}
