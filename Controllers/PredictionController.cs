using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Diagnostics;
using EnergySystemAPI.Models;
using EnergySystemAPI.Data;

[ApiController]
[Route("api/[controller]")]
public class PredictionController : ControllerBase
{
    private readonly AppDbContext _context;

    public PredictionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunPrediction([FromQuery] DateTime timestamp)
    {
        Console.WriteLine($"C# console: {timestamp}");

        // 1. Отримуємо панелі з БД
        var devices = await _context.Devices.ToListAsync();

        // 2. Отримуємо погоду з БД
        var targetTime = timestamp;         //new DateTime(2020, 5, 30, 12, 0, 0);

        var weather = await _context.WeatherData
        .Where(w => !w.IsForecast)
        .OrderBy(w => Math.Abs(EF.Functions.DateDiffSecond(w.Timestamp, targetTime)))
        .FirstOrDefaultAsync();

        // 3. Формуємо JSON для Python
        var payload = new
        {
            devices = devices.Where(d => d.DeviceTypeId == 1).Select(d => new
            {
                id = d.Id,
                name = d.Name,
                tilt = 30,
                azimuth = 180,
                area = 1.9,
                efficiency = 0.21,
                temp_coeff = -0.0035
            }),
            weather = new[] {
                new {
                    timestamp = weather.Timestamp,
                    ghi = weather.GHI,
                    temp_air = weather.Temperature,
                    wind_speed = weather.WindSpeed
                }
            },
            latitude = 49.84,
            longitude = 24.03
        };

        var jsonInput = JsonSerializer.Serialize(payload);

        // 4. Викликаємо python
        var psi = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = "Python\\PVgen.py",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        var process = Process.Start(psi);
        process.StandardInput.Write(jsonInput);
        process.StandardInput.Close();

        var output = process.StandardOutput.ReadToEnd();

        return Ok(output);
    }
}

