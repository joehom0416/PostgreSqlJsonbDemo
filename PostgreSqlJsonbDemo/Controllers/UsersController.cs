using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostgreSqlJsonbDemo.Data;
using PostgreSqlJsonbDemo.Models;
using System.Text.Json;

namespace PostgreSqlJsonbDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        user.UpdatedAt = DateTime.UtcNow;
        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await UserExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("search/profile")]
    public async Task<ActionResult<IEnumerable<User>>> SearchByProfile([FromQuery] string key, [FromQuery] string value)
    {
        var searchObject = new Dictionary<string, object> { [key] = value };
        var searchJson = JsonDocument.Parse(JsonSerializer.Serialize(searchObject));

        var users = await _context.Users
            .Where(u => EF.Functions.JsonContains(u.Profile, searchJson))
            .ToListAsync();

        return users;
    }

    [HttpGet("search/city")]
    public async Task<ActionResult<IEnumerable<User>>> SearchByCity([FromQuery] string city)
    {
        var searchObject = new Dictionary<string, object> { ["city"] = city };
        var searchJson = JsonDocument.Parse(JsonSerializer.Serialize(searchObject));

        var users = await _context.Users
            .Where(u => EF.Functions.JsonContains(u.Address, searchJson))
            .ToListAsync();

        return users;
    }

    [HttpGet("search/age-range")]
    public async Task<ActionResult<IEnumerable<User>>> SearchByAgeRange([FromQuery] int minAge, [FromQuery] int maxAge)
    {
        // For complex age range queries, we'll need to filter in memory
        // In production, consider using raw SQL for better performance
        var users = await _context.Users.ToListAsync();

        var filteredUsers = users.Where(u =>
        {
            if (u.Profile.RootElement.TryGetProperty("age", out var ageProperty) &&
                ageProperty.TryGetInt32(out var age))
            {
                return age >= minAge && age <= maxAge;
            }
            return false;
        }).ToList();

        return filteredUsers;
    }

    [HttpPatch("{id}/profile")]
    public async Task<IActionResult> UpdateProfile(int id, JsonDocument profile)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Profile = profile;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/preferences")]
    public async Task<IActionResult> UpdatePreferences(int id, JsonDocument preferences)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Preferences = preferences;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/address")]
    public async Task<IActionResult> UpdateAddress(int id, JsonDocument address)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Address = address;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<bool> UserExists(int id)
    {
        return await _context.Users.AnyAsync(e => e.Id == id);
    }
}