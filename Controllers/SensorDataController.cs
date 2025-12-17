using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Models;
using EnergySystemAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class SensorDataController : ControllerBase
{
    private readonly AppDbContext _context;

    public SensorDataController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/SensorData
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SensorData>>> GetAll()
    {
        return await _context.SensorsData.ToListAsync();
    }

    // GET: api/SensorData/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SensorData>> GetById(int id)
    {
        var item = await _context.SensorsData.FindAsync(id);

        if (item == null)
            return NotFound();

        return item;
    }

    // POST: api/SensorData
    [HttpPost]
    public async Task<ActionResult<SensorData>> Create(SensorData data)
    {
        _context.SensorsData.Add(data);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
    }

    // PUT: api/SensorData/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, SensorData data)
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
            if (!SensorDataExists(id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // DELETE: api/SensorData/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.SensorsData.FindAsync(id);

        if (item == null)
            return NotFound();

        _context.SensorsData.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SensorDataExists(int id)
    {
        return _context.SensorsData.Any(e => e.Id == id);
    }
}
