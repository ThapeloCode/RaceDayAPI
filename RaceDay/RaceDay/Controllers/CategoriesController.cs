using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Controllers;

[ApiController]
public class CategoriesController(RaceDayDbContext context) : ControllerBase
{
    [HttpGet("api/v1/events/{eventId}/categories")]
    public async Task<IActionResult> GetCategoriesForEvent(int eventId)
    {
        var categories = await context.Categories
            .Where(c => c.EventId == eventId)
            .Select(c => new CategoryResponseDto(c.Id, c.EventId, c.Title))
            .ToListAsync();
        return Ok(categories);
    }

    [HttpPost("api/v1/events/{eventId}/categories")]
    [Authorize(Roles = "Organiser,Admin")]
    public async Task<IActionResult> AddCategory(int eventId, [FromBody] CreateCategoryDto dto)
    {
        if (!await context.Events.AnyAsync(e => e.Id == eventId))
            return NotFound(new { message = "Event not found." });

        Category category = new()
        {
            EventId = eventId,
            Title = dto.Title
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategoriesForEvent), new { eventId }, new CategoryResponseDto(category.Id, category.EventId, category.Title));
    }

    [HttpPut("api/v1/categories/{id}")]
    [Authorize(Roles = "Organiser,Admin")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto dto)
    {
        var category = await context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        category.Title = dto.Title;
        await context.SaveChangesAsync();

        return Ok(new CategoryResponseDto(category.Id, category.EventId, category.Title));
    }

    [HttpDelete("api/v1/categories/{id}")]
    [Authorize(Roles = "Organiser,Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        context.Categories.Remove(category);
        await context.SaveChangesAsync();

        return Ok(new { message = "category deleted successfully" });
    }
}