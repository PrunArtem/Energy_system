using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Models;
using EnergySystemAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class MeasurementController : ControllerBase
{
    private readonly AppDbContext _context;

    public MeasurementController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Measurement
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Measurement>>> GetAll()
    {
        return await _context.Measurements.ToListAsync();
    }

    // GET: api/Measurement/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Measurement>> GetById(int id)
    {
        var item = await _context.Measurements.FindAsync(id);

        if (item == null)
            return NotFound();

        return item;
    }

    // POST: api/Measurement
    [HttpPost]
    public async Task<ActionResult<Measurement>> Create(Measurement data)
    {
        _context.Measurements.Add(data);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
    }

    // PUT: api/Measurement/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Measurement data)
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
            if (!MeasurementDataExists(id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Measurement/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Measurements.FindAsync(id);

        if (item == null)
            return NotFound();

        _context.Measurements.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MeasurementDataExists(int id)
    {
        return _context.Measurements.Any(e => e.Id == id);
    }
}
