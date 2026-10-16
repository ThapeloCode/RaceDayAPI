using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RaceDay.Controllers;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using Xunit;

namespace RaceDay.Tests;

public class AuthControllerTests
{
    private static RaceDayDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new RaceDayDbContext(options);
    }

    private static IConfiguration GetTestConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Jwt:Key", "RaceDay_SuperSecretKey_For_Jwt_Token_Signing_2026!"},
            {"Jwt:Issuer", "RaceDayApi"},
            {"Jwt:Audience", "RaceDayApp"}
        };
        return new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
    }

    [Fact]
    public async Task Register_Success_Returns_CreatedResult()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new AuthController(context, GetTestConfiguration());
        var dto = new RegisterDto("Thapelo", "Morare", "thapelo@test.com", "Password123!", "Organiser");

        // Act
        var result = await controller.Register(dto);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
        Assert.Single(context.Users);
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns_BadRequest()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Users.Add(new User { FirstName = "Existing", LastName = "User", Email = "thapelo@test.com", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var controller = new AuthController(context, GetTestConfiguration());
        var dto = new RegisterDto("Thapelo", "Morare", "thapelo@test.com", "Password123!", "Participant");

        // Act
        var result = await controller.Register(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_InvalidPassword_Returns_Unauthorized()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Users.Add(new User
        {
            FirstName = "Sipho",
            LastName = "Nkosi",
            Email = "sipho@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword")
        });
        await context.SaveChangesAsync();

        var controller = new AuthController(context, GetTestConfiguration());
        var loginDto = new LoginDto("sipho@test.com", "WrongPassword");

        // Act
        var result = await controller.Login(loginDto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}