using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace PostgreSqlJsonbDemo.Models;

public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "pending";

    public JsonDocument Items { get; set; } = JsonDocument.Parse("[]");

    public JsonDocument ShippingAddress { get; set; } = JsonDocument.Parse("{}");

    public JsonDocument PaymentInfo { get; set; } = JsonDocument.Parse("{}");

    public JsonDocument OrderHistory { get; set; } = JsonDocument.Parse("[]");

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}