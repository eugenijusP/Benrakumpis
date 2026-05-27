namespace Bebrakumpis.Application.Interfaces;

public interface IPasswordHasher
{
    bool Verify(string password, string hash);
    string Hash(string password);
}
