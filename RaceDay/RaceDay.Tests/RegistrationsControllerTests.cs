using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Controllers;
using RaceDay.Data;
using RaceDay.Models;
using Xunit;

namespace RaceDay.Tests;

public class RegistrationsControllerTests
{
    private static RaceDayDbContext GetInMemoryDbContext()
    {
        DbContextOptions<RaceDayDbContext> options = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new RaceDayDbContext(options);
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