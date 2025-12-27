using EnergySystemAPI.Models;
using EnergySystemAPI.Data;
using Microsoft.EntityFrameworkCore;

public interface ISettingsService
{
    Task<DateTime> GetSimulationStartTimeAsync();
}

public class SettingsService : ISettingsService
{
    private readonly AppDbContext _context;

    public SettingsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DateTime> GetSimulationStartTimeAsync()
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.KeyName == "SimulationStartTime");

        if (setting == null || string.IsNullOrWhiteSpace(setting.Value))
            throw new Exception("SimulationStartTime not configured");

        return DateTime.Parse(setting.Value);
    }
}
