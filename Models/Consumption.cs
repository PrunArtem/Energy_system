namespace EnergySystemAPI.Models
{
    public class Consumption
{
    public long Id { get; set; }
    public DateTime Timestamp { get; set; }
    public double DemandKWh { get; set; }
    public bool IsForecast { get; set; }
}}