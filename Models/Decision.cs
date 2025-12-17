namespace EnergySystemAPI.Models
{
    public class Decision
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string DecisionText { get; set; } = string.Empty;
        public string? DecisionType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}