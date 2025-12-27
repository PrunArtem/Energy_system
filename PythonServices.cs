using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Data;
using EnergySystemAPI.Models;
using System.Globalization;

public class PythonService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PythonService> _logger;

    public PythonService(IServiceScopeFactory scopeFactory, ILogger<PythonService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task RunPvGenerationAsync(DateTime simHour)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        _logger.LogInformation("Starting PV Python for hour {Hour}", simHour);

        var weather = await db.WeatherData
            .Where(w => w.Timestamp >= simHour &&
                        w.Timestamp < simHour.AddHours(1))
            .OrderBy(w => w.Timestamp)
            .FirstOrDefaultAsync();

        var devices = await db.Devices
            .Where(d => d.DeviceTypeId == 1)
            .Select(d => new
            {
                id = d.Id,
                name = d.Name,
                tilt = 30,
                azimuth = 180,
                area = 1.9,
                efficiency = 0.21,
                temp_coeff = -0.0035
            })
            .ToListAsync();


        if (weather == null || !devices.Any())
        {
            _logger.LogWarning("No weather or device data found. Skipping PV run.");
            return;
        }

        var payload = new
        {
            weather = new[] {
                new {
                    timestamp = weather.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    ghi = weather.GHI,
                    temp_air = weather.Temperature,
                    wind_speed = weather.WindSpeed
                }
            },
            devices = devices,
            latitude = 49.84,
            longitude = 24.03
        };

        string json = JsonSerializer.Serialize(payload);

        var psi = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = "Python\\PVGen.py",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = new Process { StartInfo = psi };

        process.Start();

        await process.StandardInput.WriteLineAsync(json);
        await process.StandardInput.FlushAsync();
        process.StandardInput.Close();

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (!string.IsNullOrWhiteSpace(error))
        {
            _logger.LogError("Python failed: {Error}", error);
            return;
        }

        if (!double.TryParse(output.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double generatedWh))
        {
            _logger.LogError("Python returned invalid number: {Out}", output);
            return;
        }

        _logger.LogInformation("Python PV Generation = {Wh} Wh - {Hour}", generatedWh, simHour);

        var normalized = simHour
    .AddMinutes(-simHour.Minute)
    .AddSeconds(-simHour.Second)
    .AddMilliseconds(-simHour.Millisecond);

var start = normalized;
var end = normalized.AddSeconds(1);

var existing = await db.Forecasts.FirstOrDefaultAsync(f =>
    f.ForecastType == "Solar" &&
    f.Timestamp.Year == start.Year &&
    f.Timestamp.Month == start.Month &&
    f.Timestamp.Day == start.Day &&
    f.Timestamp.Hour == start.Hour
);

        if (existing != null)
        {
            existing.Value = generatedWh;
            existing.LowerBound = 0;
            existing.UpperBound = 0;
            existing.Error = 0;
            existing.Source = "PVModel";
        }
        else
        {
            var record = new Forecast
            {
                Timestamp = normalized,
                ForecastType = "Solar",
                Value = generatedWh,
                LowerBound = 0,
                UpperBound = 0,
                Error = 0,
                Source = "PVModel"
            };
            db.Forecasts.Add(record);
        }
        await db.SaveChangesAsync();
    }

    public async Task RunWindGenerationAsync(DateTime simHour)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        _logger.LogInformation("Starting Wind Python for hour {Hour}", simHour);

        var weather = await db.WeatherData
            .Where(w => w.Timestamp >= simHour &&
                        w.Timestamp < simHour.AddHours(1))
            .OrderBy(w => w.Timestamp)
            .FirstOrDefaultAsync();

        var devices = await db.Devices
            .Where(d => d.DeviceTypeId == 2)
            .Select(d => new
            {
                id = d.Id,
                name = d.Name,
                ratedPower = d.RatedPower
            })
            .ToListAsync();


        if (weather == null || !devices.Any())
        {
            _logger.LogWarning("No weather or device data found. Skipping PV run.");
            return;
        }

        var payload = new
        {
            weather = new[] {
                new {
                    timestamp = weather.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    ghi = weather.GHI,
                    temp_air = weather.Temperature,
                    wind_speed = weather.WindSpeed
                }
            },
            devices = devices,
        };

        string json = JsonSerializer.Serialize(payload);

        var psi = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = "Python\\WindGen.py",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = new Process { StartInfo = psi };

        process.Start();

        await process.StandardInput.WriteLineAsync(json);
        await process.StandardInput.FlushAsync();
        process.StandardInput.Close();

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (!string.IsNullOrWhiteSpace(error))
        {
            _logger.LogError("Python failed: {Error}", error);
            return;
        }

        if (!double.TryParse(output.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double generatedWh))
        {
            _logger.LogError("Python returned invalid number: {Out}", output);
            return;
        }

        _logger.LogInformation("Python Wind Generation = {Wh} Wh - {Hour}", generatedWh, simHour);

        var normalized = simHour
    .AddMinutes(-simHour.Minute)
    .AddSeconds(-simHour.Second)
    .AddMilliseconds(-simHour.Millisecond);

var start = normalized;
var end = normalized.AddSeconds(1);

var existing = await db.Forecasts.FirstOrDefaultAsync(f =>
    f.ForecastType == "Wind" &&
    f.Timestamp.Year == start.Year &&
    f.Timestamp.Month == start.Month &&
    f.Timestamp.Day == start.Day &&
    f.Timestamp.Hour == start.Hour
);

        if (existing != null)
        {
            existing.Value = generatedWh;
            existing.LowerBound = 0;
            existing.UpperBound = 0;
            existing.Error = 0;
            existing.Source = "WindModel";
        }
        else
        {
            var record = new Forecast
            {
                Timestamp = normalized,
                ForecastType = "Wind",
                Value = generatedWh,
                LowerBound = 0,
                UpperBound = 0,
                Error = 0,
                Source = "WindModel"
            };
            db.Forecasts.Add(record);
        }
        await db.SaveChangesAsync();
    }

}
