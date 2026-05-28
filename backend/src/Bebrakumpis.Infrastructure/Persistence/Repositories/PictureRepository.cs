using System.Data;
using Bebrakumpis.Domain.Entities;
using Bebrakumpis.Domain.Interfaces;
using Bebrakumpis.Infrastructure.Persistence;
using Dapper;

namespace Bebrakumpis.Infrastructure.Persistence.Repositories;

public class PictureRepository(IDbConnectionFactory connectionFactory) : IPictureRepository
{
    public async Task<IEnumerable<Picture>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = connectionFactory.CreateConnection();
        var cmd = new CommandDefinition("""
            SELECT id, blob_url, [order], uploaded_at
            FROM pictures
            ORDER BY [order]
            """, cancellationToken: cancellationToken);
        return await connection.QueryAsync<Picture>(cmd);
    }

    public async Task<Picture?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = connectionFactory.CreateConnection();
        var cmd = new CommandDefinition("""
            SELECT id, blob_url, [order], uploaded_at
            FROM pictures
            WHERE id = @Id
            """, new { Id = id }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Picture>(cmd);
    }

    public async Task<int> GetMaxOrderAsync(CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = connectionFactory.CreateConnection();
        var cmd = new CommandDefinition("""
            SELECT COALESCE(MAX([order]), 0) FROM pictures
            """, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<int>(cmd);
    }

    public async Task<Guid> CreateAsync(Picture picture, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = connectionFactory.CreateConnection();
        var cmd = new CommandDefinition("""
            INSERT INTO pictures (id, blob_url, [order], uploaded_at)
            VALUES (@Id, @BlobUrl, @Order, @UploadedAt)
            """, picture, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(cmd);
        return picture.Id;
    }

    public async Task UpdateOrderAsync(Guid id, int order, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = connectionFactory.CreateConnection();
        var cmd = new CommandDefinition("""
            UPDATE pictures SET [order] = @Order WHERE id = @Id
            """, new { Id = id, Order = order }, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(cmd);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = connectionFactory.CreateConnection();
        var cmd = new CommandDefinition("""
            DELETE FROM pictures WHERE id = @Id
            """, new { Id = id }, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(cmd);
    }
}
