using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostgreSqlJsonbDemo.Data;
using PostgreSqlJsonbDemo.Models;
using System.Text.Json;

namespace PostgreSqlJsonbDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        product.UpdatedAt = DateTime.UtcNow;
        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ProductExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("search/specifications")]
    public async Task<ActionResult<IEnumerable<Product>>> SearchBySpecifications([FromQuery] string key, [FromQuery] string value)
    {
        var searchObject = new Dictionary<string, object> { [key] = value };
        var searchJson = JsonDocument.Parse(JsonSerializer.Serialize(searchObject));

        var products = await _context.Products
            .Where(p => EF.Functions.JsonContains(p.Specifications, searchJson))
            .ToListAsync();

        return products;
    }

    [HttpGet("search/tags")]
    public async Task<ActionResult<IEnumerable<Product>>> SearchByTag([FromQuery] string tag)
    {
        var products = await _context.Products
            .Where(p => p.Tags.Contains(tag))
            .ToListAsync();

        return products;
    }

    [HttpGet("search/price-range")]
    public async Task<ActionResult<IEnumerable<Product>>> SearchByPriceInSpecs([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        // For complex price range queries, we'll need to filter in memory
        // In production, consider using raw SQL for better performance
        var products = await _context.Products.ToListAsync();

        var filteredProducts = products.Where(p =>
        {
            if (p.Specifications.RootElement.TryGetProperty("price", out var priceProperty) &&
                priceProperty.TryGetDecimal(out var price))
            {
                return price >= minPrice && price <= maxPrice;
            }
            return false;
        }).ToList();

        return filteredProducts;
    }

    [HttpPatch("{id}/specifications")]
    public async Task<IActionResult> UpdateSpecifications(int id, JsonDocument specifications)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.Specifications = specifications;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/tags")]
    public async Task<IActionResult> AddTag(int id, [FromBody] string tag)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        if (!product.Tags.Contains(tag))
        {
            product.Tags.Add(tag);
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpDelete("{id}/tags/{tag}")]
    public async Task<IActionResult> RemoveTag(int id, string tag)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.Tags.Remove(tag);
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> ProductExists(int id)
    {
        return await _context.Products.AnyAsync(e => e.Id == id);
    }
}