using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Data;
using EnergySystemAPI.Models;

public class DecisionService
{
    private readonly AppDbContext _context;
    private SystemState state;

    public DecisionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SystemState> GetStatesAsync(double solarGeneration, double windGeneration, double consumption)
    {
        var settings = await _context.Settings
            .AsNoTracking()
            .Where(s => s.KeyName == "BatteryCharge"
                     || s.KeyName == "StabilizationMode"
                     || s.KeyName == "BatteryCriticalLevel")
            .ToListAsync();

        // ----- BatteryCharge -----
        var batterySetting = settings.FirstOrDefault(s => s.KeyName == "BatteryCharge");
        double batteryPercent = batterySetting != null
            ? double.Parse(batterySetting.Value)
            : 0;

        // ----- Critical Level -----
        var criticalSetting = settings.FirstOrDefault(s => s.KeyName == "BatteryCriticalLevel");
        double criticalLevel = criticalSetting != null
            ? double.Parse(criticalSetting.Value)
            : 20;

        // ----- Stabilization Mode / Grid -----
        var stabilizationSetting = settings.FirstOrDefault(s => s.KeyName == "StabilizationMode");
        bool isStabilizationMode = stabilizationSetting != null && stabilizationSetting.Value == "1";

        // ---- РОЗРАХУНОК СТАНІВ ----
        string batteryLevel = GetBatteryLevel(batteryPercent, criticalLevel);
        string balance = GetBalance(solarGeneration + windGeneration, consumption);

        // TODO потім заміниш на реальні значення
        bool isGridAvailable = true;
        bool isGridStable = !isStabilizationMode;

        string gridState = GetGridState(isGridAvailable, isGridStable);

        // TODO надалі заміниш реальним прогнозом
        string chargeForecast = GetChargeForecast(6);

        return new SystemState
        {
            BatteryLevel = batteryLevel,
            Balance = balance,
            GridState = gridState,
            ChargeForecast = chargeForecast
        };
    }


    public string GetBatteryLevel(double chargePercent, double criticalLimit)
    {
        if (chargePercent <= criticalLimit) return "NONE";
        if (chargePercent <= 55) return "LOW";
        if (chargePercent <= 80) return "NORMAL";
        if (chargePercent < 100) return "HIGH";
        return "FULL";
    }

    public string GetBalance(double generation, double consumption)
    {
        if (generation > consumption) return "SURPLUS";
        if (generation < consumption) return "DEFICIT";
        return "BALANCED";
    }

    public string GetGridState(bool isAvailable, bool isStable)
    {
        if (!isAvailable) return "OFFLINE";
        if (!isStable) return "UNSTABLE";
        return "AVAILABLE";
    }

    public string GetChargeForecast(double expectedChangePercent)
    {
        if (expectedChangePercent <= 0) return "NONE";
        if (expectedChangePercent <= 7) return "SLOW";
        return "FAST";
    }


    public async Task MakeDecisionsAsync(DateTime currentTime)
    {
        Console.WriteLine("1");
        string decisionText = null;
        string decisionType = null;
        Console.WriteLine("2");

        var solarGen = (await _context.Forecasts
            .Where(x => x.ForecastType == "Solar")
            .AsNoTracking()
            .ToListAsync())
            .OrderBy(e => Math.Abs((e.Timestamp - currentTime).Ticks))
            .FirstOrDefault();


        Console.WriteLine("3");

        var windGen = (await _context.Forecasts
    .Where(x => x.ForecastType == "Solar")
    .AsNoTracking()
    .ToListAsync())
    .OrderBy(e => Math.Abs((e.Timestamp - currentTime).Ticks))
    .FirstOrDefault();

        Console.WriteLine("4");

        var consumption = (await _context.Consumption
    .AsNoTracking()
    .ToListAsync())
    .OrderBy(e => Math.Abs((e.Timestamp - currentTime).Ticks))
    .FirstOrDefault();

        Console.WriteLine("5");

        state = await GetStatesAsync(solarGen.Value, windGen.Value, consumption.DemandKWh);

        Console.WriteLine("6");

        // Приклад реалізації правил за таблицею
        if (state.BatteryLevel != "FULL" && state.Balance == "SURPLUS")
        {
            decisionText = "Заряджання акумуляторів з надлишку генерації";
            decisionType = "ChargeFromSurplus";
        }
        else if (state.BatteryLevel == "NONE" && state.Balance == "DEFICIT" &&
                 (state.GridState == "AVAILABLE" || state.GridState == "UNSTABLE"))
        {
            decisionText = "Підключення до загальної мережі, заряджання акумулятора наявною генерацією";
            decisionType = "GridConnectionCharge";
        }
        else if (state.BatteryLevel == "NONE" && state.Balance == "DEFICIT" && state.GridState == "OFFLINE")
        {
            decisionText = "Агресивне енергозбереження";
            decisionType = "AggressiveSaving";
        }
        else if (state.Balance == "DEFICIT" && state.GridState == "AVAILABLE")
        {
            decisionText = "Використання акумулятора для покриття дефіциту генерації";
            decisionType = "BatteryDischarge";
        }
        else if (state.BatteryLevel == "LOW" && state.Balance == "DEFICIT" && state.GridState == "UNSTABLE" &&
                 (state.ChargeForecast == "NONE" || state.ChargeForecast == "SLOW"))
        {
            decisionText = "Підключення до загальної мережі, заряджання акумулятора наявною генерацією";
            decisionType = "GridConnectionCharge";
        }
        else if (state.BatteryLevel == "LOW" && state.Balance == "DEFICIT" && state.GridState == "UNSTABLE" &&
                 state.ChargeForecast == "FAST")
        {
            decisionText = "Використання акумулятора для покриття тимчасового дефіциту генерації";
            decisionType = "BatteryDischarge";
        }
        else if (state.BatteryLevel == "LOW" && state.Balance == "DEFICIT" && state.GridState == "OFFLINE")
        {
            decisionText = "Енергозбереження";
            decisionType = "SavingMode";
        }
        else if ((state.BatteryLevel == "NORMAL" || state.BatteryLevel == "HIGH") && state.Balance == "DEFICIT" &&
                 state.GridState == "UNSTABLE" && state.ChargeForecast == "NONE")
        {
            decisionText = "Підключення до загальної мережі, заряджання акумулятора наявною генерацією";
            decisionType = "GridConnectionCharge";
        }
        else if ((state.BatteryLevel == "NORMAL" || state.BatteryLevel == "HIGH") && state.Balance == "DEFICIT" &&
                 state.GridState == "UNSTABLE" && (state.ChargeForecast == "SLOW" || state.ChargeForecast == "FAST"))
        {
            decisionText = "Використання акумулятора для покриття тимчасового дефіциту генерації";
            decisionType = "BatteryDischarge";
        }
        else if ((state.BatteryLevel == "NORMAL" || state.BatteryLevel == "HIGH" || state.BatteryLevel == "FULL") &&
                 state.Balance == "DEFICIT" && state.GridState == "OFFLINE")
        {
            decisionText = "Використання акумулятора для покриття тимчасового дефіциту генерації";
            decisionType = "BatteryDischarge";
        }
        else if (state.BatteryLevel == "FULL" && state.Balance == "SURPLUS")
        {
            decisionText = "Продаж надлишку генерації до загальної енергосистеми";
            decisionType = "SellSurplus";
        }

        /*if (state.BatteryLevel != "FULL" && state.Balance == "SURPLUS")
        {
            decisionText = "Заряджання акумуляторів з надлишку генерації";
            decisionType = "ChargeFromSurplus";
        }
        else if (state.BatteryLevel == "NONE" && state.Balance == "DEFICIT" &&
                 (state.GridState == "AVAILABLE" || state.GridState == "UNSTABLE"))
        {
            decisionText = "Підключення до загальної мережі, заряджання акумулятора наявною генерацією";
            decisionType = "GridConnectionCharge";
        }
        else if (state.BatteryLevel == "NONE" && state.Balance == "DEFICIT" && state.GridState == "OFFLINE")
        {
            decisionText = "Агресивне енергозбереження";
            decisionType = "AggressiveSaving";
        }
        else if (state.BatteryLevel == "ANY" && state.Balance == "DEFICIT" && state.GridState == "AVAILABLE")
        {
            decisionText = "Використання акумулятора для покриття дефіциту генерації";
            decisionType = "BatteryDischarge";
        }
        else if (state.BatteryLevel == "LOW" && state.Balance == "DEFICIT" && state.GridState == "UNSTABLE" &&
                 (state.ChargeForecast == "NONE" || state.ChargeForecast == "SLOW"))
        {
            decisionText = "Підключення до загальної мережі, заряджання акумулятора наявною генерацією";
            decisionType = "GridConnectionCharge";
        }*/
        // ...додати інші правила аналогічно
        Console.WriteLine("7");

        Console.WriteLine("=====================================================");
        Console.WriteLine("=====================================================");
        Console.WriteLine("=====================================================");
        Console.WriteLine("=====================================================");
        Console.WriteLine(state.BatteryLevel); Console.WriteLine(state.Balance); Console.WriteLine(state.GridState); Console.WriteLine(state.ChargeForecast);
        Console.WriteLine("=====================================================");
        Console.WriteLine("=====================================================");
        Console.WriteLine("=====================================================");
        Console.WriteLine("=====================================================");
        // Якщо знайдено рішення, зберегти його у базу
        if (!string.IsNullOrEmpty(decisionText))
        {
            var decision = new Decision
            {
                Timestamp = currentTime,
                DecisionText = decisionText,
                DecisionType = decisionType
            };

            _context.Decisions.Add(decision);
            await _context.SaveChangesAsync();
        }
    }
}

