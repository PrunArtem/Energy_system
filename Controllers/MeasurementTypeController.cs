using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Models;
using EnergySystemAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class MeasurementTypeController : ControllerBase
{
    private readonly AppDbContext _context;

    public MeasurementTypeController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/MeasurementType
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MeasurementType>>> GetAll()
    {
        return await _context.MeasurementTypes.ToListAsync();
    }

    // GET: api/MeasurementType/5
    [HttpGet("{id}")]
    public async Task<ActionResult<MeasurementType>> GetById(int id)
    {
        var item = await _context.MeasurementTypes.FindAsync(id);

        if (item == null)
            return NotFound();

        return item;
    }

    // POST: api/MeasurementType
    [HttpPost]
    public async Task<ActionResult<MeasurementType>> Create(MeasurementType data)
    {
        _context.MeasurementTypes.Add(data);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
    }

    // PUT: api/MeasurementType/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, MeasurementType data)
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
            if (!MeasurementTypeDataExists(id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // DELETE: api/MeasurementType/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.MeasurementTypes.FindAsync(id);

        if (item == null)
            return NotFound();

        _context.MeasurementTypes.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MeasurementTypeDataExists(int id)
    {
        return _context.MeasurementTypes.Any(e => e.Id == id);
    }
}
