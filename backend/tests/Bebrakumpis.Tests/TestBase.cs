using System.Data;
using Bebrakumpis.Domain.Entities;
using Bebrakumpis.Infrastructure.Persistence;
using Bebrakumpis.Infrastructure.Services;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bebrakumpis.Tests;

public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
        => parameter.Value = value.ToString();

    public override Guid Parse(object value)
        => Guid.Parse((string)value);
}

public class TestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid():N}";
    private SqliteConnection? _keepAlive;

    public async Task InitializeAsync()
    {
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
        // Keep-alive connection prevents the in-memory DB from disappearing
        _keepAlive = new SqliteConnection($"DataSource={_dbName};Mode=Memory;Cache=Shared");
        await _keepAlive.OpenAsync();
        await InitSchemaAsync(_keepAlive);
    }

    private static async Task InitSchemaAsync(SqliteConnection connection)
    {
        await connection.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS _migrations (
                migration_name TEXT NOT NULL PRIMARY KEY,
                applied_at     TEXT NOT NULL DEFAULT (datetime('now'))
            );
            CREATE TABLE IF NOT EXISTS users (
                id            TEXT NOT NULL PRIMARY KEY,
                username      TEXT NOT NULL UNIQUE,
                password_hash TEXT NOT NULL,
                role          TEXT NOT NULL DEFAULT 'User',
                created_at    TEXT NOT NULL DEFAULT (datetime('now'))
            );
            """);
    }

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbConnectionFactory));
            if (descriptor is not null) services.Remove(descriptor);
            services.AddScoped<IDbConnectionFactory>(_ =>
                new SqliteConnectionFactory($"DataSource={_dbName};Mode=Memory;Cache=Shared"));

            var migrationDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(Bebrakumpis.API.Migrations.MigrationRunner));
            if (migrationDescriptor is not null) services.Remove(migrationDescriptor);
            services.AddScoped<Bebrakumpis.API.Migrations.MigrationRunner, NoOpMigrationRunner>();
        });
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "test-secret-key-for-integration-tests-min32chars!",
                ["Jwt:Issuer"] = "Bebrakumpis",
                ["Jwt:Audience"] = "Bebrakumpis"
            });
        });
    }

    public async Task<User> SeedUserAsync(string username = "testuser", string role = "User", string password = "Test@123")
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role,
            CreatedAt = DateTime.UtcNow
        };
        await _keepAlive!.ExecuteAsync(
            "INSERT INTO users (id, username, password_hash, role, created_at) VALUES (@Id, @Username, @PasswordHash, @Role, @CreatedAt)",
            user);
        return user;
    }

    public new async Task DisposeAsync()
    {
        if (_keepAlive is not null)
        {
            await _keepAlive.DisposeAsync();
        }
        await base.DisposeAsync();
    }
}

public class SqliteConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public System.Data.IDbConnection CreateConnection() =>
        new SqliteConnection(connectionString);
}

public class NoOpMigrationRunner(
    IDbConnectionFactory connectionFactory,
    Microsoft.Extensions.Logging.ILogger<Bebrakumpis.API.Migrations.MigrationRunner> logger)
    : Bebrakumpis.API.Migrations.MigrationRunner(connectionFactory, logger)
{
    public override Task RunAsync() => Task.CompletedTask;
}
