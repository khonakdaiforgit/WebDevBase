// MyApp.WebAPI.Tests/Unit/AuthControllerUnitTests.cs
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyApp.Application.Abstractions.Users;
using MyApp.Application.Abstractions.Users.Dtos;
using MyApp.Domain.Entities;
using MyApp.WebAPI.Controllers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace MyApp.WebAPI.Tests.Unit;

public class AuthControllerUnitTests
{
    private readonly AuthController _controller;
    private readonly Mock<IAuthService> _authServiceMock;

    public AuthControllerUnitTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_Valid_ReturnsAuthResult()
    {
        // Arrange
        var dto = new LoginDto("admin@example.com", "Password123!");
        var expected = new AuthResult("jwt", "refresh", DateTime.UtcNow.AddHours(1),
            new UserDto(Guid.NewGuid(), "admin@example.com", UserRole.Admin, true, DateTime.UtcNow));

        _authServiceMock.Setup(x => x.LoginAsync(dto)).ReturnsAsync(expected);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.ExpiresAt));
    }

    [Fact]
    public async Task ChangePassword_WithValidToken_CallsService()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()) };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
            }
        };

        var dto = new ChangePasswordDto("old", "new123");
        _authServiceMock.Setup(x => x.ChangePasswordAsync(userId, dto)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ChangePassword(dto);

        // Assert
        result.Should().BeOfType<OkResult>();
        _authServiceMock.Verify(x => x.ChangePasswordAsync(userId, dto), Times.Once);
    }
}