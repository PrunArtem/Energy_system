using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Models;
using EnergySystemAPI.Data;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ISimulationClock _clock;

    public SettingsController(AppDbContext context, ISimulationClock clock)
    {
        _context = context;
        _clock = clock;
    }

    [HttpGet("{key}")]
    public async Task<ActionResult<string>> Get(string key)
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.KeyName == key);

        if (setting == null)
            return NotFound();

        return setting.Value;
    }

    [HttpPost]
    public async Task<IActionResult> Set(Setting model)
    {
        var existing = await _context.Settings
            .FirstOrDefaultAsync(s => s.KeyName == model.KeyName);

        if (existing == null)
        {
            _context.Settings.Add(model);
        }
        else
        {
            existing.Value = model.Value;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Ok(model);
    }

    [HttpPost("refresh")]
    public IActionResult Refresh()
    {
        _clock.RunManualRefresh();
        return Ok("Refresh triggered");
    }

        [HttpGet("battery")]
    public IActionResult GetBatteryCharge()
    {
        var battery = _context.Settings.FirstOrDefault(s => s.KeyName == "BatteryCharge");
        if (battery == null) return NotFound("BatteryCharge not found");
        return Ok(int.Parse(battery.Value));
    }

    [HttpPost("battery/{value:int}")]
    public async Task<IActionResult> SetBatteryCharge(int value)
    {
        if (value < 0 || value > 100)
            return BadRequest("Battery must be between 0–100");

        var battery = _context.Settings.FirstOrDefault(s => s.KeyName == "BatteryCharge");
        if (battery == null)
        {
            battery = new Setting
            {
                KeyName = "BatteryCharge",
                Value = value.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Settings.Add(battery);
        }
        else
        {
            battery.Value = value.ToString();
            battery.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Ok(value);
    }


    // ========= EMERGENCY / STABILIZATION MODE ==========

    [HttpGet("mode")]
    public IActionResult GetStabilizationMode()
    {
        var mode = _context.Settings.FirstOrDefault(s => s.KeyName == "StabilizationMode");
        if (mode == null) return NotFound("StabilizationMode not found");

        // 0 – вимкнено, 1 – увімкнено
        return Ok(int.Parse(mode.Value));
    }

    [HttpPost("mode/{state:int}")]
    public async Task<IActionResult> SetStabilizationMode(int state)
    {
        if (state != 0 && state != 1)
            return BadRequest("Mode must be 0 or 1");

        var mode = _context.Settings.FirstOrDefault(s => s.KeyName == "StabilizationMode");

        if (mode == null)
        {
            mode = new Setting
            {
                KeyName = "StabilizationMode",
                Value = state.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Settings.Add(mode);
        }
        else
        {
            mode.Value = state.ToString();
            mode.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Ok(state);
    }

}