using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Controllers;

[ApiController]
[Route("api/v1/events")]
public class EventsController(RaceDayDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetEvents()
    {
        var events = await context.Events
            .Select(e => new EventResponseDto(e.Id, e.Title, e.Description, e.StartDate, e.EndDate, e.Location))
            .ToListAsync();
        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        var evt = await context.Events.FindAsync(id);
        if (evt == null) return NotFound();
        return Ok(new EventResponseDto(evt.Id, evt.Title, evt.Description, evt.StartDate, evt.EndDate, evt.Location));
    }

    [HttpPost]
    [Authorize(Roles = "Organiser,Admin")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
    {
        Event evt = new()
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Location = dto.Location
        };

        context.Events.Add(evt);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEvent), new { id = evt.Id }, new EventResponseDto(evt.Id, evt.Title, evt.Description, evt.StartDate, evt.EndDate, evt.Location));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Organiser,Admin,Participant")]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] CreateEventDto dto)
    {
        var evt = await context.Events.FindAsync(id);
        if (evt == null) return NotFound();

        evt.Title = dto.Title;
        evt.Description = dto.Description;
        evt.StartDate = dto.StartDate;
        evt.EndDate = dto.EndDate;
        evt.Location = dto.Location;

        await context.SaveChangesAsync();
        return Ok(new EventResponseDto(evt.Id, evt.Title, evt.Description, evt.StartDate, evt.EndDate, evt.Location));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organiser,Admin")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var evt = await context.Events.FindAsync(id);
        if (evt == null) return NotFound();

        context.Events.Remove(evt);
        await context.SaveChangesAsync();
        return Ok(new { message = "Event deleted successfully" });
    }
}