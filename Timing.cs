using System.Threading.Tasks;
using EnergySystemAPI.Models;

public interface ISimulationClock
{
    DateTime Now { get; }
    void SetTime(DateTime newTime);

    void Tick();
    void RunManualRefresh();
}

public class SimulationClock : ISimulationClock
{
    private DateTime _startSimTime;
    private readonly DateTime _realStart;
    private readonly PythonService _pythonService;
    private readonly DecisionService _decisionService;
    private readonly IServiceScopeFactory _scopeFactory;

    

    private Timer _timer;

        public SimulationClock(DateTime startSimTime, IServiceScopeFactory scopeFactory)
    {
        _startSimTime = startSimTime;
        _scopeFactory = scopeFactory;
        _realStart = DateTime.Now;
        _timer = new Timer(_ =>
        {
            var passed = DateTime.Now - _realStart;
            var _currentTime = _startSimTime + passed;

            if (_lastTriggeredHour != _currentTime.Hour)
            {
                _lastTriggeredHour = _currentTime.Hour;
                RunHourlyJobs(_currentTime);
            }
        }, null, 0, 1000);
    }

    /*public SimulationClock(DateTime startSimTime, PythonService pythonService, DecisionService decisionService)
    {
        _pythonService = pythonService;
        _decisionService = decisionService;
        _startSimTime = startSimTime;
        _realStart = DateTime.Now;
        _timer = new Timer(_ =>
        {
            var passed = DateTime.Now - _realStart;
            var _currentTime = _startSimTime + passed;

            if (_lastTriggeredHour != _currentTime.Hour)
            {
                _lastTriggeredHour = _currentTime.Hour;
                RunHourlyJobs(_currentTime);
            }
        }, null, 0, 1000);
    }*/

    public DateTime Now
    {
        get
        {
            var passed = DateTime.Now - _realStart;
            return _startSimTime + passed;
        }
    }
    public void SetTime(DateTime newTime) => _startSimTime = newTime;

    private static int? _lastTriggeredHour = null;

    public void Tick()
    {
        var passed = DateTime.Now - _realStart;
        var _currentTime = _startSimTime + passed;

        if (_lastTriggeredHour != _currentTime.Hour)
        {
            _lastTriggeredHour = _currentTime.Hour;
            RunHourlyJobs(_currentTime);
        }
    }

    private async Task RunHourlyJobs(DateTime currentTime)
    {
        using var scope = _scopeFactory.CreateScope();

        var python = scope.ServiceProvider.GetRequiredService<PythonService>();
        var decision = scope.ServiceProvider.GetRequiredService<DecisionService>();

        await decision.MakeDecisionsAsync(currentTime);
        

        for (int i = 0; i < 12; i++)
        {
            //_ = _pythonService.RunPvGenerationAsync(currentTime.AddHours(i));
            //_ = _pythonService.RunWindGenerationAsync(currentTime.AddHours(i));
            await python.RunPvGenerationAsync(currentTime.AddHours(i));
            await python.RunWindGenerationAsync(currentTime.AddHours(i));
            Console.WriteLine("=====================================");
        }
        Console.WriteLine("a");
    //await _decisionService.MakeDecisionsAsync(currentTime); 
    

Console.WriteLine("b");
    }

    public void RunManualRefresh()
    {
    var passed = DateTime.Now - _realStart;
    var currentTime = _startSimTime + passed;

    RunHourlyJobs(currentTime);
    }

}
