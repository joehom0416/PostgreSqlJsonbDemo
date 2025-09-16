using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostgreSqlJsonbDemo.Data;
using PostgreSqlJsonbDemo.Models;
using System.Text.Json;

namespace PostgreSqlJsonbDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        return await _context.Orders.Include(o => o.User).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _context.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
        {
            return NotFound();
        }
        return order;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, Order order)
    {
        if (id != order.Id)
        {
            return BadRequest();
        }

        order.UpdatedAt = DateTime.UtcNow;
        _context.Entry(order).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await OrderExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("search/status")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByStatus([FromQuery] string status)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Where(o => o.Status == status)
            .ToListAsync();
    }

    [HttpGet("search/items")]
    public async Task<ActionResult<IEnumerable<Order>>> SearchOrdersByProduct([FromQuery] string productName)
    {
        var searchItem = new { name = productName };
        var searchArray = new[] { searchItem };
        var searchJson = JsonDocument.Parse(JsonSerializer.Serialize(searchArray));

        var orders = await _context.Orders
            .Include(o => o.User)
            .Where(o => EF.Functions.JsonContains(o.Items, searchJson))
            .ToListAsync();

        return orders;
    }

    [HttpGet("search/total-range")]
    public async Task<ActionResult<IEnumerable<Order>>> SearchByTotalRange([FromQuery] decimal minTotal, [FromQuery] decimal maxTotal)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Where(o => o.TotalAmount >= minTotal && o.TotalAmount <= maxTotal)
            .ToListAsync();
    }

    [HttpGet("search/shipping-city")]
    public async Task<ActionResult<IEnumerable<Order>>> SearchByShippingCity([FromQuery] string city)
    {
        var searchObject = new Dictionary<string, object> { ["city"] = city };
        var searchJson = JsonDocument.Parse(JsonSerializer.Serialize(searchObject));

        var orders = await _context.Orders
            .Include(o => o.User)
            .Where(o => EF.Functions.JsonContains(o.ShippingAddress, searchJson))
            .ToListAsync();

        return orders;
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var oldStatus = order.Status;
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        var historyEntryObject = new
        {
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            from = oldStatus,
            to = status
        };
        var historyEntry = JsonDocument.Parse(JsonSerializer.Serialize(historyEntryObject));

        var currentHistory = JsonSerializer.Deserialize<JsonElement[]>(order.OrderHistory.RootElement.GetRawText()) ?? Array.Empty<JsonElement>();
        var newHistory = currentHistory.Append(historyEntry.RootElement).ToArray();
        order.OrderHistory = JsonDocument.Parse(JsonSerializer.Serialize(newHistory));

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/items")]
    public async Task<IActionResult> UpdateOrderItems(int id, JsonDocument items)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        order.Items = items;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/history")]
    public async Task<ActionResult<JsonDocument>> GetOrderHistory(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        return order.OrderHistory;
    }

    private async Task<bool> OrderExists(int id)
    {
        return await _context.Orders.AnyAsync(e => e.Id == id);
    }
}