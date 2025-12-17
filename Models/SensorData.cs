namespace EnergySystemAPI.Models
{
    public class SensorData
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public float SolarPower { get; set; }
        public float Consumption { get; set; }
        public float BatteryLevel { get; set; }
    }
}
