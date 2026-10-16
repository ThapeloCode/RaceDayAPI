using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Controllers;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using Xunit;

namespace RaceDay.Tests;

public class ApiEndpointTests
{
    private static RaceDayDbContext GetInMemoryDbContext()
    {
        DbContextOptions<RaceDayDbContext> options = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new RaceDayDbContext(options);
    }

    [Fact]
    public async Task CreateEvent_Returns_CreatedAtAction_With_Event_Details()
    {
        // Arrange
        using RaceDayDbContext context = GetInMemoryDbContext();
        EventsController controller = new(context);
        CreateEventDto dto = new("Soweto Marathon", "Annual road race", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), "Soweto");

        // Act
        IActionResult result = await controller.CreateEvent(dto);

        // Assert
        CreatedAtActionResult actionResult = Assert.IsType<CreatedAtActionResult>(result);
        EventResponseDto eventDto = Assert.IsType<EventResponseDto>(actionResult.Value);
        Assert.Equal("Soweto Marathon", eventDto.Title);
        Assert.Equal("Soweto", eventDto.Location);
    }

    [Fact]
    public async Task GetCategoryLeaderboard_Returns_Results_Ordered_By_Fastest_Time()
    {
        // Arrange
        using RaceDayDbContext context = GetInMemoryDbContext();
        ResultsController controller = new(context);

        User user1 = new() { FirstName = "Thapelo", LastName = "Morare", Email = "thapelo@example.com" };
        User user2 = new() { FirstName = "Sipho", LastName = "Nkosi", Email = "sipho@example.com" };
        context.Users.AddRange(user1, user2);
        await context.SaveChangesAsync();

        Registration reg1 = new() { UserId = user1.Id, CategoryId = 1, BibNumber = "101" };
        Registration reg2 = new() { UserId = user2.Id, CategoryId = 1, BibNumber = "102" };
        context.Registrations.AddRange(reg1, reg2);
        await context.SaveChangesAsync();

        context.Results.Add(new Result { RegistrationId = reg1.Id, FinishTimeSeconds = 3600 });
        context.Results.Add(new Result { RegistrationId = reg2.Id, FinishTimeSeconds = 3200 });
        await context.SaveChangesAsync();

        // Act
        IActionResult result = await controller.GetCategoryLeaderboard(1);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<ResultResponseDto> leaderboard = Assert.IsType<List<ResultResponseDto>>(okResult.Value);

        Assert.Equal(2, leaderboard.Count);
        Assert.Equal(3200, leaderboard[0].FinishTimeSeconds); // Fastest runner first
        Assert.Equal("Sipho Nkosi", leaderboard[0].ParticipantName);
    }

    [Fact]
    public async Task CancelRegistration_Removes_Registration_From_Database()
    {
        // Arrange
        using RaceDayDbContext context = GetInMemoryDbContext();
        RegistrationsController controller = new(context);

        Registration reg = new() { UserId = 1, CategoryId = 2, BibNumber = "202", Date = DateTime.UtcNow };
        context.Registrations.Add(reg);
        await context.SaveChangesAsync();

        // Act
        IActionResult result = await controller.CancelRegistration(reg.Id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Empty(context.Registrations);
    }
}