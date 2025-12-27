namespace EnergySystemAPI.Models
{
    public class Device
    {
        public int Id { get; set; }
        public int DeviceTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public double? RatedPower { get; set; }
        public DateTime CreatedAt { get; set; }

    }

}