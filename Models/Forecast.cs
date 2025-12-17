namespace EnergySystemAPI.Models
{
    public class Forecast
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string ForecastType { get; set; } = string.Empty;
        public double Value { get; set; }
        public double? LowerBound { get; set; }
        public double? UpperBound { get; set; }
        public double? Error { get; set; }
        public string? Source { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}