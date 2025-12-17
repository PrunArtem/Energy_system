namespace EnergySystemAPI.Models
{
    public class Measurement
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int MeasurementTypeId { get; set; }
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }

    }
}