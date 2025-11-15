// MyApp.WebAPI.Tests/Integration/AuthIntegrationTests.cs
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.VisualStudio.TestPlatform.TestHost;
using MyApp.Application.Abstractions.Users;
using MyApp.Application.Abstractions.Users.Dtos;
using System.Net;
using System.Net.Http.Json;

namespace MyApp.WebAPI.Tests.Integration;

public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.UseContentRoot(projectDir);

            builder.ConfigureServices(services =>
            {
                services.AddScoped<IAuthService, TestAuthService>();
            });
        }).CreateClient();
    }

    [Fact]
    public async Task Login_Valid_ReturnsTokenAndUser()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto("admin@example.com", "Password123!"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResult>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeEmpty();
        result.User.Email.Should().Be("admin@example.com");
    }
}