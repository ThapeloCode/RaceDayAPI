using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using Route = RaceDay.Models.Route;

namespace RaceDay.Controllers;

[ApiController]
public class RoutesController(RaceDayDbContext context) : ControllerBase
{
    [HttpGet("api/categories/{categoryId}/route")]
    public async Task<IActionResult> GetRouteForCategory(int categoryId)
    {
        var route = await context.Routes
            .Where(r => r.CategoryId == categoryId)
            .Select(r => new RouteResponseDto(r.Id, r.CategoryId, r.Title, r.Location, r.DistanceKm))
            .FirstOrDefaultAsync();

        if (route == null) return NotFound(new { message = "No route found for this category." });
        return Ok(route);
    }

    [HttpPost("api/categories/{categoryId}/route")]
    [Authorize(Roles = "Organiser,Admin")]
    public async Task<IActionResult> SetRoute(int categoryId, [FromBody] CreateRouteDto dto)
    {
        var existingRoute = await context.Routes.FirstOrDefaultAsync(r => r.CategoryId == categoryId);
        if (existingRoute != null)
        {
            existingRoute.Title = dto.Title;
            existingRoute.Location = dto.Location;
            existingRoute.DistanceKm = dto.DistanceKm;
        }
        else
        {
            Route route = new()
            {
                CategoryId = categoryId,
                Title = dto.Title,
                Location = dto.Location,
                DistanceKm = dto.DistanceKm
            };
            context.Routes.Add(route);
        }

        await context.SaveChangesAsync();
        var result = await context.Routes.FirstAsync(r => r.CategoryId == categoryId);
        return CreatedAtAction(nameof(GetRouteForCategory), new { categoryId }, new RouteResponseDto(result.Id, result.CategoryId, result.Title, result.Location, result.DistanceKm));
    }

    [HttpPut("api/routes/{id}")]
    [Authorize(Roles = "Organiser,Admin")]
    public async Task<IActionResult> UpdateRoute(int id, [FromBody] CreateRouteDto dto)
    {
        var route = await context.Routes.FindAsync(id);
        if (route == null) return NotFound();

        route.Title = dto.Title;
        route.Location = dto.Location;
        route.DistanceKm = dto.DistanceKm;

        await context.SaveChangesAsync();
        return Ok(new RouteResponseDto(route.Id, route.CategoryId, route.Title, route.Location, route.DistanceKm));
    }
}