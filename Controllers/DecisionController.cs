using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Models;
using EnergySystemAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class DecisionController : ControllerBase
{
    private readonly AppDbContext _context;

    public DecisionController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Decision
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Decision>>> GetAll()
    {
        return await _context.Decisions.ToListAsync();
    }

    // GET: api/Decision/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Decision>> GetById(int id)
    {
        var item = await _context.Decisions.FindAsync(id);

        if (item == null)
            return NotFound();

        return item;
    }

    // POST: api/Decision
    [HttpPost]
    public async Task<ActionResult<Decision>> Create(Decision data)
    {
        _context.Decisions.Add(data);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
    }

    // PUT: api/Decision/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Decision data)
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
            if (!DecisionExists(id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Decision/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Decisions.FindAsync(id);

        if (item == null)
            return NotFound();

        _context.Decisions.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DecisionExists(int id)
    {
        return _context.Decisions.Any(e => e.Id == id);
    }
}
