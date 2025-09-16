using Microsoft.AspNetCore.Mvc;
using PostgreSqlJsonbDemo.Data;
using PostgreSqlJsonbDemo.Models;
using System.Text.Json;

namespace PostgreSqlJsonbDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DemoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("seed-data")]
    public async Task<IActionResult> SeedData()
    {
        if (_context.Users.Any() || _context.Products.Any())
        {
            return BadRequest("Database already contains data. Clear it first.");
        }

        var users = new List<User>
        {
            new User
            {
                Email = "john.doe@example.com",
                Name = "John Doe",
                Profile = JsonDocument.Parse(@"{
                    ""age"": 30,
                    ""gender"": ""male"",
                    ""occupation"": ""Software Engineer"",
                    ""interests"": [""technology"", ""gaming"", ""reading""]
                }"),
                Preferences = JsonDocument.Parse(@"{
                    ""theme"": ""dark"",
                    ""language"": ""en"",
                    ""notifications"": {
                        ""email"": true,
                        ""push"": false,
                        ""sms"": true
                    }
                }"),
                Address = JsonDocument.Parse(@"{
                    ""street"": ""123 Main St"",
                    ""city"": ""San Francisco"",
                    ""state"": ""CA"",
                    ""zipCode"": ""94101"",
                    ""country"": ""USA""
                }")
            },
            new User
            {
                Email = "jane.smith@example.com",
                Name = "Jane Smith",
                Profile = JsonDocument.Parse(@"{
                    ""age"": 28,
                    ""gender"": ""female"",
                    ""occupation"": ""Product Manager"",
                    ""interests"": [""design"", ""travel"", ""photography""]
                }"),
                Preferences = JsonDocument.Parse(@"{
                    ""theme"": ""light"",
                    ""language"": ""en"",
                    ""notifications"": {
                        ""email"": true,
                        ""push"": true,
                        ""sms"": false
                    }
                }"),
                Address = JsonDocument.Parse(@"{
                    ""street"": ""456 Oak Ave"",
                    ""city"": ""New York"",
                    ""state"": ""NY"",
                    ""zipCode"": ""10001"",
                    ""country"": ""USA""
                }")
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        var products = new List<Product>
        {
            new Product
            {
                Name = "MacBook Pro 16\"",
                Price = 2499.99m,
                Specifications = JsonDocument.Parse(@"{
                    ""cpu"": ""Apple M2 Max"",
                    ""ram"": ""32GB"",
                    ""storage"": ""1TB SSD"",
                    ""display"": ""16.2-inch Liquid Retina XDR"",
                    ""weight"": ""2.15 kg"",
                    ""color"": ""Space Gray"",
                    ""ports"": [""3x Thunderbolt 4"", ""HDMI"", ""SD card"", ""MagSafe 3""]
                }"),
                Metadata = JsonDocument.Parse(@"{
                    ""manufacturer"": ""Apple"",
                    ""category"": ""laptop"",
                    ""releaseDate"": ""2023-01-17"",
                    ""warranty"": ""1 year"",
                    ""inStock"": true,
                    ""sku"": ""MBP16-M2MAX-32-1TB""
                }"),
                Tags = new List<string> { "laptop", "apple", "premium", "professional" }
            },
            new Product
            {
                Name = "Samsung Galaxy S23 Ultra",
                Price = 1199.99m,
                Specifications = JsonDocument.Parse(@"{
                    ""cpu"": ""Snapdragon 8 Gen 2"",
                    ""ram"": ""12GB"",
                    ""storage"": ""256GB"",
                    ""display"": ""6.8-inch Dynamic AMOLED 2X"",
                    ""camera"": {
                        ""main"": ""200MP"",
                        ""ultrawide"": ""12MP"",
                        ""telephoto"": [""10MP"", ""10MP""],
                        ""front"": ""12MP""
                    },
                    ""battery"": ""5000mAh"",
                    ""color"": ""Phantom Black""
                }"),
                Metadata = JsonDocument.Parse(@"{
                    ""manufacturer"": ""Samsung"",
                    ""category"": ""smartphone"",
                    ""releaseDate"": ""2023-02-17"",
                    ""warranty"": ""1 year"",
                    ""inStock"": true,
                    ""sku"": ""SGS23U-12-256-PB""
                }"),
                Tags = new List<string> { "smartphone", "samsung", "android", "flagship" }
            },
            new Product
            {
                Name = "Sony WH-1000XM5",
                Price = 399.99m,
                Specifications = JsonDocument.Parse(@"{
                    ""type"": ""Over-ear wireless headphones"",
                    ""driver"": ""30mm"",
                    ""frequency"": ""4Hz-40kHz"",
                    ""batteryLife"": ""30 hours"",
                    ""noiseCancellation"": true,
                    ""bluetooth"": ""5.2"",
                    ""weight"": ""250g"",
                    ""color"": ""Black""
                }"),
                Metadata = JsonDocument.Parse(@"{
                    ""manufacturer"": ""Sony"",
                    ""category"": ""audio"",
                    ""releaseDate"": ""2022-05-12"",
                    ""warranty"": ""1 year"",
                    ""inStock"": true,
                    ""sku"": ""WH1000XM5-B""
                }"),
                Tags = new List<string> { "headphones", "sony", "wireless", "noise-cancelling" }
            }
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        var orders = new List<Order>
        {
            new Order
            {
                UserId = users[0].Id,
                TotalAmount = 2899.98m,
                Status = "shipped",
                Items = JsonDocument.Parse(@"[
                    {
                        ""productId"": " + products[0].Id + @",
                        ""name"": ""MacBook Pro 16\"""",
                        ""price"": 2499.99,
                        ""quantity"": 1
                    },
                    {
                        ""productId"": " + products[2].Id + @",
                        ""name"": ""Sony WH-1000XM5"",
                        ""price"": 399.99,
                        ""quantity"": 1
                    }
                ]"),
                ShippingAddress = JsonDocument.Parse(@"{
                    ""street"": ""123 Main St"",
                    ""city"": ""San Francisco"",
                    ""state"": ""CA"",
                    ""zipCode"": ""94101"",
                    ""country"": ""USA""
                }"),
                PaymentInfo = JsonDocument.Parse(@"{
                    ""method"": ""credit_card"",
                    ""last4"": ""1234"",
                    ""type"": ""visa""
                }"),
                OrderHistory = JsonDocument.Parse(@"[
                    {
                        ""timestamp"": ""2024-01-15T10:00:00Z"",
                        ""status"": ""created"",
                        ""note"": ""Order placed""
                    },
                    {
                        ""timestamp"": ""2024-01-15T14:30:00Z"",
                        ""status"": ""confirmed"",
                        ""note"": ""Payment processed""
                    },
                    {
                        ""timestamp"": ""2024-01-16T09:00:00Z"",
                        ""status"": ""shipped"",
                        ""note"": ""Order shipped via UPS"",
                        ""tracking"": ""1Z999AA1234567890""
                    }
                ]")
            }
        };

        _context.Orders.AddRange(orders);

        var logs = new List<LogEntry>
        {
            new LogEntry
            {
                Level = "info",
                Message = "User logged in successfully",
                Data = JsonDocument.Parse(@"{
                    ""userId"": " + users[0].Id + @",
                    ""userAgent"": ""Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)"",
                    ""ipAddress"": ""192.168.1.100""
                }"),
                Context = JsonDocument.Parse(@"{
                    ""sessionId"": ""sess_abc123"",
                    ""requestId"": ""req_xyz789"",
                    ""source"": ""authentication-service""
                }")
            },
            new LogEntry
            {
                Level = "error",
                Message = "Payment processing failed",
                Data = JsonDocument.Parse(@"{
                    ""orderId"": " + orders[0].Id + @",
                    ""amount"": 2899.98,
                    ""errorCode"": ""CARD_DECLINED"",
                    ""retryCount"": 2
                }"),
                Context = JsonDocument.Parse(@"{
                    ""sessionId"": ""sess_def456"",
                    ""requestId"": ""req_abc123"",
                    ""source"": ""payment-service""
                }")
            },
            new LogEntry
            {
                Level = "warning",
                Message = "High memory usage detected",
                Data = JsonDocument.Parse(@"{
                    ""memoryUsage"": 85.5,
                    ""threshold"": 80.0,
                    ""service"": ""user-service""
                }"),
                Context = JsonDocument.Parse(@"{
                    ""server"": ""prod-server-01"",
                    ""region"": ""us-west-2"",
                    ""source"": ""monitoring-service""
                }")
            }
        };

        _context.LogEntries.AddRange(logs);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Demo data seeded successfully",
            counts = new
            {
                users = users.Count,
                products = products.Count,
                orders = orders.Count,
                logs = logs.Count
            }
        });
    }

    [HttpDelete("clear-data")]
    public async Task<IActionResult> ClearData()
    {
        _context.LogEntries.RemoveRange(_context.LogEntries);
        _context.Orders.RemoveRange(_context.Orders);
        _context.Products.RemoveRange(_context.Products);
        _context.Users.RemoveRange(_context.Users);

        await _context.SaveChangesAsync();

        return Ok(new { message = "All data cleared successfully" });
    }

    [HttpGet("examples")]
    public IActionResult GetExamples()
    {
        return Ok(new
        {
            message = "PostgreSQL JSONB Demo Examples",
            examples = new
            {
                jsonb_queries = new
                {
                    description = "Examples of JSONB query capabilities",
                    queries = new[]
                    {
                        new
                        {
                            name = "Search products by specification",
                            endpoint = "GET /api/products/search/specifications?key=cpu&value=Apple M2 Max",
                            description = "Find products with specific CPU using JSONB contains"
                        },
                        new
                        {
                            name = "Search users by age range",
                            endpoint = "GET /api/users/search/age-range?minAge=25&maxAge=35",
                            description = "Find users within age range using JSONB path extraction"
                        },
                        new
                        {
                            name = "Search orders by shipping city",
                            endpoint = "GET /api/orders/search/shipping-city?city=San Francisco",
                            description = "Find orders shipping to specific city"
                        },
                        new
                        {
                            name = "Search logs by error code",
                            endpoint = "GET /api/logs/search/data?key=errorCode&value=CARD_DECLINED",
                            description = "Find log entries with specific error codes"
                        }
                    }
                },
                jsonb_operations = new
                {
                    description = "Examples of JSONB manipulation operations",
                    operations = new[]
                    {
                        new
                        {
                            name = "Update user profile",
                            endpoint = "PATCH /api/users/{id}/profile",
                            description = "Update entire JSONB profile field"
                        },
                        new
                        {
                            name = "Add product tag",
                            endpoint = "POST /api/products/{id}/tags",
                            description = "Add tag to JSONB array field"
                        },
                        new
                        {
                            name = "Update order status with history",
                            endpoint = "PATCH /api/orders/{id}/status",
                            description = "Update status and append to JSONB history array"
                        }
                    }
                },
                advanced_features = new
                {
                    description = "Advanced JSONB features demonstrated",
                    features = new[]
                    {
                        "GIN indexes on JSONB columns for fast queries",
                        "JSONB containment operators (@>, <@)",
                        "JSONB path extraction (->>, #>>)",
                        "JSONB array operations",
                        "JSONB aggregation and analytics",
                        "Partial JSONB updates"
                    }
                }
            }
        });
    }
}