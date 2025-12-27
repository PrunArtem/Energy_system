using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Models;
using EnergySystemAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class ConsumptionController : ControllerBase
{
    private readonly AppDbContext _context;

    public ConsumptionController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/DeviceType
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Consumption>>> GetAll()
    {
        return await _context.Consumption.ToListAsync();
    }

    // GET: api/DeviceType/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Consumption>> GetById(int id)
    {
        var item = await _context.Consumption.FindAsync(id);

        if (item == null)
            return NotFound();

        return item;
    }

    // POST: api/DeviceType
    [HttpPost]
    public async Task<ActionResult<Consumption>> Create(Consumption data)
    {
        _context.Consumption.Add(data);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
    }

    // PUT: api/DeviceType/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Consumption data)
    {
        if (id != data.Id)
            return BadRequest("ID in URL does not match ID in body");

        _context.Entry(data).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ConsumptionExists(id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // DELETE: api/DeviceType/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Consumption.FindAsync(id);

        if (item == null)
            return NotFound();

        _context.Consumption.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ConsumptionExists(int id)
    {
        return _context.Consumption.Any(e => e.Id == id);
    }
}
