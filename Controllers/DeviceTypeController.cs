using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Models;
using EnergySystemAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class DeviceTypeController : ControllerBase
{
    private readonly AppDbContext _context;

    public DeviceTypeController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/DeviceType
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceType>>> GetAll()
    {
        return await _context.DeviceTypes.ToListAsync();
    }

    // GET: api/DeviceType/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DeviceType>> GetById(int id)
    {
        var item = await _context.DeviceTypes.FindAsync(id);

        if (item == null)
            return NotFound();

        return item;
    }

    // POST: api/DeviceType
    [HttpPost]
    public async Task<ActionResult<DeviceType>> Create(DeviceType data)
    {
        _context.DeviceTypes.Add(data);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
    }

    // PUT: api/DeviceType/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, DeviceType data)
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
            if (!DeviceTypeExists(id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // DELETE: api/DeviceType/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.DeviceTypes.FindAsync(id);

        if (item == null)
            return NotFound();

        _context.DeviceTypes.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DeviceTypeExists(int id)
    {
        return _context.DeviceTypes.Any(e => e.Id == id);
    }
}
