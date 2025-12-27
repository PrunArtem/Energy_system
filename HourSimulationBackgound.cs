public class HourlySimulationWorker : BackgroundService
{
    private readonly ISimulationClock _clock;
    private int? _lastTriggeredHour = null;

    public HourlySimulationWorker(ISimulationClock clock)
    {
        _clock = clock;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var t = _clock.Now;

            if (t.Minute == 0 && t.Second == 0)
            {
                if (_lastTriggeredHour != t.Hour)
                {
                    _lastTriggeredHour = t.Hour;
                    RunHourlyJobs(t);
                }
            }

            await Task.Delay(1000, token);   // 1 реальна секунда = скільки завгодно симуляційного часу
        }
    }

    private void RunHourlyJobs(DateTime simTime)
    {
        Console.WriteLine($"Hourly job triggered at {simTime}");

        // 🔥 тут викликаєш свої обрахунки:
        // - прогноз
        // - енергобаланс
        // - заряд акумулятора
    }
}
