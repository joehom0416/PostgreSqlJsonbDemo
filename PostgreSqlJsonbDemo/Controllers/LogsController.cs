using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostgreSqlJsonbDemo.Data;
using PostgreSqlJsonbDemo.Models;
using System.Text.Json;

namespace PostgreSqlJsonbDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LogsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LogEntry>>> GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var logs = await _context.LogEntries
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return logs;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LogEntry>> GetLog(int id)
    {
        var log = await _context.LogEntries.FindAsync(id);
        if (log == null)
        {
            return NotFound();
        }
        return log;
    }

    [HttpPost]
    public async Task<ActionResult<LogEntry>> CreateLog(LogEntry log)
    {
        log.Timestamp = DateTime.UtcNow;

        _context.LogEntries.Add(log);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLog), new { id = log.Id }, log);
    }

    [HttpGet("search/level/{level}")]
    public async Task<ActionResult<IEnumerable<LogEntry>>> GetLogsByLevel(string level)
    {
        return await _context.LogEntries
            .Where(l => l.Level == level)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
    }

    [HttpGet("search/message")]
    public async Task<ActionResult<IEnumerable<LogEntry>>> SearchLogsByMessage([FromQuery] string query)
    {
        return await _context.LogEntries
            .Where(l => l.Message.Contains(query))
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
    }

    [HttpGet("search/data")]
    public async Task<ActionResult<IEnumerable<LogEntry>>> SearchLogsByData([FromQuery] string key, [FromQuery] string value)
    {
        var searchObject = new Dictionary<string, object> { [key] = value };
        var searchJson = JsonDocument.Parse(JsonSerializer.Serialize(searchObject));

        var logs = await _context.LogEntries
            .Where(l => EF.Functions.JsonContains(l.Data, searchJson))
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();

        return logs;
    }

    [HttpGet("search/context")]
    public async Task<ActionResult<IEnumerable<LogEntry>>> SearchLogsByContext([FromQuery] string key, [FromQuery] string value)
    {
        var searchObject = new Dictionary<string, object> { [key] = value };
        var searchJson = JsonDocument.Parse(JsonSerializer.Serialize(searchObject));

        var logs = await _context.LogEntries
            .Where(l => EF.Functions.JsonContains(l.Context, searchJson))
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();

        return logs;
    }

    [HttpGet("search/daterange")]
    public async Task<ActionResult<IEnumerable<LogEntry>>> GetLogsByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        return await _context.LogEntries
            .Where(l => l.Timestamp >= from && l.Timestamp <= to)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
    }

    [HttpGet("analytics/errors")]
    public async Task<ActionResult<object>> GetErrorAnalytics()
    {
        var errorLogs = await _context.LogEntries
            .Where(l => l.Level == "error")
            .ToListAsync();

        var errorsByHour = errorLogs
            .GroupBy(l => new { l.Timestamp.Date, l.Timestamp.Hour })
            .Select(g => new
            {
                Date = g.Key.Date,
                Hour = g.Key.Hour,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ThenBy(x => x.Hour);

        return new
        {
            TotalErrors = errorLogs.Count,
            ErrorsByHour = errorsByHour,
            RecentErrors = errorLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(10)
                .Select(l => new { l.Id, l.Message, l.Timestamp })
        };
    }

    [HttpDelete("cleanup")]
    public async Task<IActionResult> CleanupOldLogs([FromQuery] int daysOld = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
        var oldLogs = await _context.LogEntries
            .Where(l => l.Timestamp < cutoffDate)
            .ToListAsync();

        _context.LogEntries.RemoveRange(oldLogs);
        await _context.SaveChangesAsync();

        return Ok(new { DeletedCount = oldLogs.Count });
    }
}