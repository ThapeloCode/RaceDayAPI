using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Controllers;

[ApiController]
public class RegistrationsController(RaceDayDbContext context) : ControllerBase
{
    [HttpPost("api/v1/registrations")]
    [Authorize]
    public async Task<IActionResult> CreateRegistration([FromBody] CreateRegistrationDto dto)
    {
        Registration reg = new()
        {
            UserId = dto.UserId,
            CategoryId = dto.CategoryId,
            BibNumber = dto.BibNumber,
            Date = DateTime.UtcNow
        };

        context.Registrations.Add(reg);
        await context.SaveChangesAsync();

        return Created($"/api/v1/registrations/{reg.Id}", new RegistrationResponseDto(reg.Id, reg.UserId, reg.CategoryId, reg.BibNumber, reg.Date));
    }

    [HttpGet("api/v1/users/{userId}/registrations")]
    [Authorize]
    public async Task<IActionResult> GetUserRegistrations(int userId)
    {
        var list = await context.Registrations
            .Where(r => r.UserId == userId)
            .Select(r => new RegistrationResponseDto(r.Id, r.UserId, r.CategoryId, r.BibNumber, r.Date))
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("api/v1/categories/{categoryId}/registrations")]
    [Authorize(Roles = "Organiser,Admin")]
    public async Task<IActionResult> GetCategoryRegistrations(int categoryId)
    {
        var list = await context.Registrations
            .Where(r => r.CategoryId == categoryId)
            .Select(r => new RegistrationResponseDto(r.Id, r.UserId, r.CategoryId, r.BibNumber, r.Date))
            .ToListAsync();
        return Ok(list);
    }

    [HttpDelete("api/v1/registrations/{id}")]
    [Authorize]
    public async Task<IActionResult> CancelRegistration(int id)
    {
        var reg = await context.Registrations.FindAsync(id);
        if (reg == null) return NotFound();

        context.Registrations.Remove(reg);
        await context.SaveChangesAsync();

        return Ok(new { message = "Registration Cancelled" });
    }
}