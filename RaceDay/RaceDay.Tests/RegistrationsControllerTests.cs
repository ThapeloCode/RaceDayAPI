using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Controllers;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using Xunit;

namespace RaceDay.Tests;

public class RegistrationsControllerTests
{
    private static RaceDayDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new RaceDayDbContext(options);
    }

    [Fact]
    public async Task Participant_Can_Enrol_And_Enrolment_Is_Recorded()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new RegistrationsController(context);
        var dto = new CreateRegistrationDto(UserId: 1, CategoryId: 10, BibNumber: "BIB-500");

        // Act
        var result = await controller.CreateRegistration(dto);

        // Assert
        Assert.IsType<CreatedResult>(result);
        Assert.Single(context.Registrations);
        Assert.Equal("BIB-500", context.Registrations.First().BibNumber);
    }

    [Fact]
    public async Task CancelRegistration_Removes_Registration_From_Database()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new RegistrationsController(context);
        var reg = new Registration { UserId = 1, CategoryId = 2, BibNumber = "202", Date = DateTime.UtcNow };
        context.Registrations.Add(reg);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.CancelRegistration(reg.Id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Empty(context.Registrations);
    }
}