using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(RaceDayDbContext context, IConfiguration config) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (await context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest(new { message = "Email already registered." });

        User user = new()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = string.IsNullOrWhiteSpace(dto.Role) ? "Participant" : dto.Role
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        return CreatedAtAction(nameof(GetMe), null, new
        {
            id = user.Id,
            first_name = user.FirstName,
            last_name = user.LastName,
            email = user.Email,
            role = user.Role,
            token
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        var token = GenerateJwtToken(user);
        return Ok(new { token, user = new { id = user.Id, email = user.Email, role = user.Role } });
    }

    [HttpGet("users")]
    [Authorize(Roles = "Organiser,Admin")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await context.Users
            .Select(u => new UserResponseDto(u.Id, u.FirstName, u.LastName, u.Email, u.Role))
            .ToListAsync();
        return Ok(users);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        return Ok(new UserResponseDto(user.Id, user.FirstName, user.LastName, user.Email, user.Role));
    }

    [HttpGet("/api/v1/users/{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null) return NotFound();

        return Ok(new UserResponseDto(user.Id, user.FirstName, user.LastName, user.Email, user.Role));
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        ];

        JwtSecurityToken token = new(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}