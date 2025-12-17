using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Models;
using EnergySystemAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class ForecastController : ControllerBase
{
    private readonly AppDbContext _context;

    public ForecastController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Forecast
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Forecast>>> GetAll()
    {
        return await _context.Forecasts.ToListAsync();
    }

    // GET: api/Forecast/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Forecast>> GetById(int id)
    {
        var item = await _context.Forecasts.FindAsync(id);

        if (item == null)
            return NotFound();

        return item;
    }

    // POST: api/Forecast
    [HttpPost]
    public async Task<ActionResult<Forecast>> Create(Forecast data)
    {
        _context.Forecasts.Add(data);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
    }

    // PUT: api/Forecast/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Forecast data)
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
            if (!ForecastExists(id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Forecast/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Forecasts.FindAsync(id);

        if (item == null)
            return NotFound();

        _context.Forecasts.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ForecastExists(int id)
    {
        return _context.Forecasts.Any(e => e.Id == id);
    }
}
