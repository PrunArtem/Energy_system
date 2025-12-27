import pandas as pd
from sqlalchemy import create_engine

# === 1. Підключення до MySQL ===
DB_HOST ="localhost"
DB_USER ="root"
DB_PASS ="32151"
DB_NAME ="energy_system"

csv_path = "E:\\Code Files\\EVmodeling\\consumption\\data_hourly.csv"

print("Loading CSV...")
df = pd.read_csv(csv_path)

df = df[df["ID"] == "Exp_43"]

# only required columns
df = df[['From', 'Demand_kWh']]

# drop NA
df = df.dropna(subset=['Demand_kWh'])

# convert time
df['Timestamp'] = pd.to_datetime(df['From'], utc=True)

# filter 2020–2021
df = df[(df['Timestamp'].dt.year >= 2020) & (df['Timestamp'].dt.year <= 2021)]

# rename
df = df.rename(columns={'Demand_kWh': 'DemandKWh'})

# add IsForecast
df['IsForecast'] = False

# final columns order
df = df[['Timestamp', 'DemandKWh', 'IsForecast']]

print(f"Records to insert: {len(df)}")

print("Connecting to DB...")
engine = create_engine(f"mysql+pymysql://{DB_USER}:{DB_PASS}@{DB_HOST}/{DB_NAME}")

print("Writing to MySQL...")
df.to_sql("consumption", engine, if_exists="append", index=False)

print("Done 🎉")