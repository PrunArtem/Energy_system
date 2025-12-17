using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Models;
using EnergySystemAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class WeatherDataController : ControllerBase
{
    private readonly AppDbContext _context;

    public WeatherDataController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/WeatherData
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeatherData>>> GetAll()
    {
        return await _context.WeatherData.ToListAsync();
    }

    // GET: api/WeatherData/5
    [HttpGet("{id}")]
    public async Task<ActionResult<WeatherData>> GetById(int id)
    {
        var item = await _context.WeatherData.FindAsync(id);

        if (item == null)
            return NotFound();

        return item;
    }

    // POST: api/WeatherData
    [HttpPost]
    public async Task<ActionResult<WeatherData>> Create(WeatherData data)
    {
        _context.WeatherData.Add(data);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
    }

    // PUT: api/WeatherData/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, WeatherData data)
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
            if (!WeatherDataExists(id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // DELETE: api/WeatherData/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.WeatherData.FindAsync(id);

        if (item == null)
            return NotFound();

        _context.WeatherData.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool WeatherDataExists(int id)
    {
        return _context.WeatherData.Any(e => e.Id == id);
    }
}
