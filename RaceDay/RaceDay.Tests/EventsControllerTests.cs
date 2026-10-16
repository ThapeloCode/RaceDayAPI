using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Controllers;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using Xunit;

namespace RaceDay.Tests;

public class EventsControllerTests
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
}