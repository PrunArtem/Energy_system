import pandas as pd

def fill_missing_with_zeros(input_path: str, output_path: str):
    # Завантажуємо CSV
    df = pd.read_csv(input_path)

    # Замінюємо всі NaN / None / порожні строки на 0
    df = df.fillna(0)

    # Якщо є порожні строки (""), замінюємо і їх
    df = df.replace('', 0)

    # Зберігаємо результат
    df.to_csv(output_path, index=False)


if __name__ == "__main__":
    fill_missing_with_zeros("E:\\EnergySystem\\EnergySystemAPI\\Data\\data_cleaned.csv", "weather_clean.csv")
