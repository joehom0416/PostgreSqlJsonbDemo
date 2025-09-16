using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace PostgreSqlJsonbDemo.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public JsonDocument Specifications { get; set; } = JsonDocument.Parse("{}");

    public JsonDocument Metadata { get; set; } = JsonDocument.Parse("{}");

    public List<string> Tags { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}