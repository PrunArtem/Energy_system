import numpy as np
import sys
import json
import pandas as pd

# Таблиця характеристик
wind_speeds = np.array([0, 1.3, 3, 5, 7, 9, 10, 11, 12, 15, 20, 25])
power_output = np.array([0, 0, 150, 450, 900, 1700, 2200, 2600, 3000, 3200, 3300, 3300])

def vawt_power(v):
    """
    Повертає потужність (Вт) для заданої швидкості вітру v (м/с)
    на основі апроксимованої power curve VAWT.
    """
    return float(np.interp(v, wind_speeds, power_output))


raw = sys.stdin.read()
data = json.loads(raw)

# Convert weather
weather = pd.DataFrame(data["weather"])
weather["timestamp"] = pd.to_datetime(weather["timestamp"])
weather = weather.set_index("timestamp")

weather = weather.rename(columns={
    "wind_speed": "wind_speed"
})

# ---------- CALCULATION ----------
total_generation_wh = 0.0   # Wh total for all devices

for device in data["devices"]:

    # Instant power for this panel (in W)
    power = vawt_power(weather["wind_speed"])

    # Sum energy for the day (Wh)
    energy_wh = power.sum() * 1  # якщо дані кожні 1 хвилину (залежить від частоти!)

    total_generation_wh += energy_wh

# ---------- OUTPUT ----------
# Output only total generation as a number
print(total_generation_wh)
