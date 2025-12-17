using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Models;

namespace EnergySystemAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Decision> Decisions { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<Forecast> Forecasts { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<MeasurementType> MeasurementTypes { get; set; }
        public DbSet<SensorData> SensorsData { get; set; }
        public DbSet<WeatherData> WeatherData { get; set; }
    }
}
