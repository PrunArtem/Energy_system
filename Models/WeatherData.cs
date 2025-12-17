namespace EnergySystemAPI.Models
{
    public class WeatherData
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double? Temperature { get; set; }
        public double? FeelsLike { get; set; }
        public double? Humidity { get; set; }
        public double? WindSpeed { get; set; }
        public double? WindDirection { get; set; }
        public double? GHI { get; set; }
        public double? Cloudiness { get; set; }
        public double? Pressure { get; set; }
        public double? Precipitation { get; set; }
        public bool IsForecast { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}