namespace EnergySystemAPI.Models
{
    public class Setting
    {
        public int Id { get; set; }
        public string KeyName { get; set; } = null!;
        public string Value { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}