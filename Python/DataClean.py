import csv
import mysql.connector
from datetime import datetime

# === 1. Підключення до MySQL ===
db = mysql.connector.connect(
    host="localhost",
    user="root",
    password="32151",
    database="energy_system",
)

cursor = db.cursor()

# === 2. Відкриваємо CSV ===
with open("E:\\EnergySystem\\EnergySystemAPI\\Data\\weather_clean.csv", "r", encoding="utf-8") as file:
    reader = csv.DictReader(file, delimiter=';')

    for row in reader:
        # === 3. Перетворюємо дату у MySQL формат ===
        ts = datetime.strptime(row["reference_timestamp"], "%d.%m.%Y %H:%M")

        # === 4. Читаємо дані ===
        temperature = float(row["Air temperature 2 m above ground hourly mean"])
        humidity = float(row["Relative air humidity 2 m above ground hourly mean"])
        pressure = float(row["Atmospheric pressure at barometric altitude (QFE) hourly mean"])
        wind_dir = float(row["Wind direction hourly mean"])
        wind_speed = float(row["Wind speed scalar hourly mean in m/s"])   # Використовуємо m/s
        precipitation = float(row["Precipitation hourly total"])
        ghi = float(row["Global radiation hourly mean"])

        # === 5. Формуємо INSERT ===
        cursor.execute("""
            INSERT INTO weatherdata
            (Timestamp, Temperature, Humidity, WindDirection, WindSpeed, Precipitation, Pressure,
             GHI, Cloudiness, FeelsLike, IsForecast, CreatedAt)
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s, 0, 0, 0, NOW());
        """, (
            ts, temperature, humidity, wind_dir, wind_speed, precipitation, pressure,
            ghi
        ))

db.commit()
cursor.close()
db.close()

print("Готово! Дані імпортовано.")
