import time
import mysql.connector
import adafruit_dht
import board
from adafruit_bmp280 import Adafruit_BMP280_I2C
from w1thermsensor import W1ThermSensor
from datetime import datetime, timedelta
import logging

# Konfiguracja logowania
logging.basicConfig(
    filename="/var/log/weather_station.log",
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s"
)
logging.info("Weather station service started.")

# Połączenie z bazą danych MySQL
db_connection = mysql.connector.connect(
    host="localhost",
    user="weather_user",
    password="strong_password",
    database="weather_station"
)
cursor = db_connection.cursor()

# Inicjalizacja czujnikow
dht22 = adafruit_dht.DHT22(board.D27)  # czujnik DHT22 na pinie D27
bmp280 = Adafruit_BMP280_I2C(board.I2C(), address=0x76)  # czujnik BMP280 z adresem 0x76
ds18b20 = W1ThermSensor()  # czujnik DS18B20

# Znacznik czasowy startu programu
start_time = datetime.now()

# Flaga zapisu dziennego
daily_save_done = False

# Funkcja do zapisu danych do tabeli Last24Hours
def save_to_last24hours(temp_inside, temp_outside, humidity, pressure):
    query = """
    INSERT INTO Last24Hours (temperature_inside, temperature_outside, humidity, pressure)
    VALUES (%s, %s, %s, %s)
    """
    values = (temp_inside, temp_outside, humidity, pressure)
    cursor.execute(query, values)
    db_connection.commit()
    logging.info("Data saved to Last24Hours.")

# Funkcja do zapisu danych do tabeli Last31Days
def save_to_last31days(avg_temp_inside, avg_temp_outside, avg_humidity, avg_pressure):
    query = """
    INSERT INTO Last31Days (avg_temp_inside, avg_temp_outside, avg_humidity, avg_pressure)
    VALUES (%s, %s, %s, %s)
    """
    values = (avg_temp_inside, avg_temp_outside, avg_humidity, avg_pressure)
    cursor.execute(query, values)
    db_connection.commit()
    logging.info("Averages saved to Last31Days.")

# Funkcja do zbierania danych z czujnikow
def get_sensor_data():
    try:
        temp_outside = ds18b20.get_temperature()
        time.sleep(2)
        humidity = dht22.humidity
        temp_inside = dht22.temperature
        pressure = bmp280.pressure + 22
        return temp_inside, temp_outside, humidity, pressure
    except Exception as e:
        logging.error("Error reading sensors.", exc_info=True)
        return None, None, None, None

# Funkcja do obliczeń średnich wartości dla Last31Days
def get_avg_data():
    query = "SELECT AVG(temperature_inside), AVG(temperature_outside), AVG(humidity), AVG(pressure) FROM Last24Hours"
    cursor.execute(query)
    result = cursor.fetchone()
    logging.info("Averages calculated.")
    return result

# Funkcja do usuwania starych danych z Last24Hours
def delete_old_data():
    query = "DELETE FROM Last24Hours WHERE timestamp < NOW() - INTERVAL 24 HOUR"
    cursor.execute(query)
    db_connection.commit()
    logging.info("Old data deleted from Last24Hours.")

# Główna pętla, zapis co 30 sekund do Last24Hours
while True:
    temp_inside, temp_outside, humidity, pressure = get_sensor_data()

    # Zapis danych co 30 sekund
    if temp_inside is not None and temp_outside is not None and humidity is not None and pressure is not None:
        save_to_last24hours(temp_inside, temp_outside, humidity, pressure)

    # Co dzień o północy, zapis średnich danych do Last31Days
    current_time = datetime.now()
    if current_time.hour == 0 and current_time.minute == 0 and not daily_save_done:
        avg_temp_inside, avg_temp_outside, avg_humidity, avg_pressure = get_avg_data()
        if avg_temp_inside is not None:
            save_to_last31days(avg_temp_inside, avg_temp_outside, avg_humidity, avg_pressure)
        daily_save_done = True
    elif current_time.hour != 0:
        daily_save_done = False

    # Usuwanie danych starszych niż 24 godziny
    delete_old_data()

    # Czekanie 15 sekund
    time.sleep(15)
