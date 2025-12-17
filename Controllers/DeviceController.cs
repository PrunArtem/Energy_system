using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Models;
using EnergySystemAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class DeviceController : ControllerBase
{
    private readonly AppDbContext _context;

    public DeviceController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Device
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Device>>> GetAll()
    {
        return await _context.Devices.ToListAsync();
    }

    // GET: api/Device/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Device>> GetById(int id)
    {
        var item = await _context.Devices.FindAsync(id);

        if (item == null)
            return NotFound();

        return item;
    }

    // POST: api/Device
    [HttpPost]
    public async Task<ActionResult<Device>> Create(Device data)
    {
        _context.Devices.Add(data);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
    }

    // PUT: api/Device/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Device data)
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
            if (!DeviceExists(id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Device/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Devices.FindAsync(id);

        if (item == null)
            return NotFound();

        _context.Devices.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DeviceExists(int id)
    {
        return _context.Devices.Any(e => e.Id == id);
    }
}
