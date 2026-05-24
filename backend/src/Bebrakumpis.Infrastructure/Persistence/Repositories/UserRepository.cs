using System.Data;
using Bebrakumpis.Domain.Entities;
using Bebrakumpis.Domain.Interfaces;
using Bebrakumpis.Infrastructure.Persistence;
using Dapper;

namespace Bebrakumpis.Infrastructure.Persistence.Repositories;

public class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>("""
            SELECT id, username, password_hash, role, created_at
            FROM users
            WHERE username = @Username
            """, new { Username = username });
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>("""
            SELECT id, username, password_hash, role, created_at
            FROM users
            WHERE id = @Id
            """, new { Id = id });
    }
}
