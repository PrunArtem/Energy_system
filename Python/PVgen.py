import sys
import json
import ast
import pandas as pd
import pvlib
from pvlib.location import Location

# ---------- READ INPUT ----------
#raw = sys.stdin.read().strip()
#data = json.loads(raw)
raw = sys.stdin.read()

raw = raw.encode("utf-8").decode("utf-8-sig").strip()

data = ast.literal_eval(raw)

latitude = data["latitude"]
longitude = data["longitude"]

# Convert weather
weather = pd.DataFrame(data["weather"])
weather["timestamp"] = pd.to_datetime(weather["timestamp"])
weather = weather.set_index("timestamp")

weather = weather.rename(columns={
    "ghi": "GHI",
    "temp_air": "temp_air",
    "wind_speed": "wind_speed"
})

# ---------- LOCATION ----------
loc = Location(latitude, longitude)

# ---------- SOLAR POSITION ----------
solpos = loc.get_solarposition(weather.index)
weather["zenith"] = solpos["zenith"]

# ---------- ESTIMATE DNI + DHI ----------
disc_out = pvlib.irradiance.disc(weather["GHI"], weather["zenith"], weather.index)
weather["DNI"] = disc_out["dni"]

weather["DHI"] = weather["GHI"] - weather["DNI"] * pvlib.tools.cosd(weather["zenith"])

# ---------- CALCULATION ----------
total_generation_wh = 0.0   # Wh total for all devices

for device in data["devices"]:
    tilt = device["tilt"]
    azimuth = device["azimuth"]
    area = device["area"]
    eff = device["efficiency"]
    temp_coeff = device["temp_coeff"]

    poa = pvlib.irradiance.get_total_irradiance(
        surface_tilt=tilt,
        surface_azimuth=azimuth,
        dni=weather["DNI"],
        ghi=weather["GHI"],
        dhi=weather["DHI"],
        solar_zenith=weather["zenith"],
        solar_azimuth=solpos["azimuth"]
    )

    irr = poa["poa_global"]  # W/m2

    temp_cell = pvlib.temperature.sapm_cell(
        irr,
        weather["temp_air"],
        weather["wind_speed"],
        -3.56,          #Surface temperature calculation parameters
        -0.075,         #Based on the type of a solar panel
        3,              #
        1000            #
    )

    # Instant power for this panel (in W)
    power = area * eff * irr * (1 + temp_coeff * (temp_cell - 25))

    # Sum energy for the day (Wh)
    energy_wh = power.sum() * 1  # якщо дані кожні 1 хвилину (залежить від частоти!)

    total_generation_wh += energy_wh

# ---------- OUTPUT ----------
# Output only total generation as a number
print(total_generation_wh)
