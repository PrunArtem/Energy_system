namespace EnergySystemAPI.Models
{
    public class MeasurementType
    {
        public int Id { get; set; }
        public int DeviceTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public string? Description { get; set; }

    }
}