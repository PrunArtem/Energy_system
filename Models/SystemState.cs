namespace EnergySystemAPI.Models
{public class SystemState
{
    public string BatteryLevel { get; set; }      // NONE, LOW, NORMAL, HIGH, FULL
    public string Balance { get; set; }           // SURPLUS, DEFICIT
    public string GridState { get; set; }         // ANY, AVAILABLE, UNSTABLE, OFFLINE
    public string ChargeForecast { get; set; }    // ANY, NONE, SLOW, FAST
}}
