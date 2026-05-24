using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Bebrakumpis.Tests.Controllers;

public class AuthControllerTests : IClassFixture<TestWebAppFactory>, IAsyncLifetime
{
    private readonly TestWebAppFactory _factory;
    private readonly HttpClient _client;

    public AuthControllerTests(TestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Login_ShouldSetCookieAndReturn200_WhenCredentialsAreValid()
    {
        await _factory.SeedUserAsync("loginuser", "User", "Test@123");

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new { username = "loginuser", password = "Test@123" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));
        var cookie = response.Headers.GetValues("Set-Cookie").First();
        Assert.Contains("bh_auth", cookie);
        Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_ShouldReturn401_WhenCredentialsAreInvalid()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new { username = "nonexistent", password = "wrong" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturn400_WhenRequestBodyIsEmpty()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new { username = "", password = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Logout_ShouldReturn200_AndClearCookie()
    {
        var response = await _client.PostAsync("/api/v1/auth/logout", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Me_ShouldReturn401_WhenNotAuthenticated()
    {
        using var anonClient = _factory.CreateClient();
        var response = await anonClient.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_ShouldReturnUserInfo_WhenAuthenticated()
    {
        await _factory.SeedUserAsync("meuser", "Admin", "Test@123");

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new { username = "meuser", password = "Test@123" });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var meResponse = await _client.GetAsync("/api/v1/auth/me");
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

        var body = await meResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("meuser", body.GetProperty("username").GetString());
        Assert.Equal("Admin", body.GetProperty("role").GetString());
    }
}
