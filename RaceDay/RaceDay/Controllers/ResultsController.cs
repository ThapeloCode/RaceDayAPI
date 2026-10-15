using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Controllers;

[ApiController]
public class ResultsController(RaceDayDbContext context) : ControllerBase
{
    [HttpPost("api/registrations/{registrationId}/results")]
    [Authorize(Roles = "Organiser,Admin")]
    public async Task<IActionResult> RecordResult(int registrationId, [FromBody] RecordResultDto dto)
    {
        var reg = await context.Registrations.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == registrationId);
        if (reg == null) return NotFound(new { message = "Registration not found." });

        var result = await context.Results.FirstOrDefaultAsync(r => r.RegistrationId == registrationId);
        if (result == null)
        {
            result = new Result
            {
                RegistrationId = registrationId,
                FinishTimeSeconds = dto.FinishTimeSeconds
            };
            context.Results.Add(result);
        }
        else
        {
            result.FinishTimeSeconds = dto.FinishTimeSeconds;
        }

        await context.SaveChangesAsync();
        return Created("", new ResultResponseDto(result.Id, result.RegistrationId, result.FinishTimeSeconds, $"{reg.User.FirstName} {reg.User.LastName}", reg.BibNumber));
    }

    [HttpGet("api/categories/{categoryId}/results")]
    public async Task<IActionResult> GetCategoryLeaderboard(int categoryId)
    {
        var leaderboard = await context.Results
            .Include(r => r.Registration)
            .ThenInclude(reg => reg.User)
            .Where(r => r.Registration.CategoryId == categoryId)
            .OrderBy(r => r.FinishTimeSeconds)
            .Select(r => new ResultResponseDto(
                r.Id,
                r.RegistrationId,
                r.FinishTimeSeconds,
                $"{r.Registration.User.FirstName} {r.Registration.User.LastName}",
                r.Registration.BibNumber
            ))
            .ToListAsync();

        return Ok(leaderboard);
    }

    [HttpGet("api/users/{userId}/results")]
    [Authorize]
    public async Task<IActionResult> GetUserResultsHistory(int userId)
    {
        var history = await context.Results
            .Include(r => r.Registration)
            .ThenInclude(reg => reg.User)
            .Where(r => r.Registration.UserId == userId)
            .Select(r => new ResultResponseDto(
                r.Id,
                r.RegistrationId,
                r.FinishTimeSeconds,
                $"{r.Registration.User.FirstName} {r.Registration.User.LastName}",
                r.Registration.BibNumber
            ))
            .ToListAsync();

        return Ok(history);
    }

    [HttpDelete("api/results/{id}")]
    [Authorize(Roles = "Organiser,Admin")]
    public async Task<IActionResult> UpdateResult(int id, [FromBody] RecordResultDto dto)
    {
        var result = await context.Results.Include(r => r.Registration).ThenInclude(reg => reg.User).FirstOrDefaultAsync(r => r.Id == id);
        if (result == null) return NotFound();

        result.FinishTimeSeconds = dto.FinishTimeSeconds;
        await context.SaveChangesAsync();

        return Ok(new ResultResponseDto(result.Id, result.RegistrationId, result.FinishTimeSeconds, $"{result.Registration.User.FirstName} {result.Registration.User.LastName}", result.Registration.BibNumber));
    }
}