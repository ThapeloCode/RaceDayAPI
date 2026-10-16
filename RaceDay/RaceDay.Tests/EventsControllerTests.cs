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
        var options = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new RaceDayDbContext(options);
    }

    [Fact]
    public async Task Organiser_Can_Create_Event_Successfully()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new EventsController(context);
        var dto = new CreateEventDto("Soweto Marathon", "Annual road race", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), "Soweto");

        // Act
        var result = await controller.CreateEvent(dto);

        // Assert
        var actionResult = Assert.IsType<CreatedAtActionResult>(result);
        var eventDto = Assert.IsType<EventResponseDto>(actionResult.Value);
        Assert.Equal("Soweto Marathon", eventDto.Title);
    }

    [Fact]
    public async Task GetEvent_NonExistentId_Returns_NotFound()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new EventsController(context);

        // Act
        var result = await controller.GetEvent(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}