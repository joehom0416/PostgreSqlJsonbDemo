using System.Text.Json;

namespace PostgreSqlJsonbDemo.Models;

public class LogEntry
{
    public int Id { get; set; }

    public string Level { get; set; } = "info";

    public string Message { get; set; } = string.Empty;

    public JsonDocument Data { get; set; } = JsonDocument.Parse("{}");

    public JsonDocument Context { get; set; } = JsonDocument.Parse("{}");

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}