using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace PostgreSqlJsonbDemo.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public JsonDocument Profile { get; set; } = JsonDocument.Parse("{}");

    public JsonDocument Preferences { get; set; } = JsonDocument.Parse("{}");

    public JsonDocument Address { get; set; } = JsonDocument.Parse("{}");

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}