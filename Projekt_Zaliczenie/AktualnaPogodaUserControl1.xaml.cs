using System;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;  // Importujemy MySqlConnector

namespace Projekt_Zaliczenie
{
    public partial class AktualnaPogodaUserControl1 : UserControl
    {
        // Zmieniamy connection string na używający MySqlConnector
        private string connectionString = "Server=raspberrypi;Database=weather_station;Uid=weather_user;Pwd=strong_password;";

        public AktualnaPogodaUserControl1()
        {
            InitializeComponent();
            LoadWeatherData();
        }

        public async void LoadWeatherData()
        {
            string query = "SELECT timestamp, temperature_inside, temperature_outside, humidity, pressure FROM Last24Hours ORDER BY timestamp DESC LIMIT 1;";

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync(); // Używamy asynchronicznego otwarcia połączenia
                    //MessageBox.Show("Połączenie z bazą danych powiodło się!");

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync()) // Używamy asynchronicznego odczytu
                        {
                            if (await reader.ReadAsync()) // Asynchroniczny sposób odczytu wiersza
                            {
                                DateTextBox.Text = reader.GetDateTime("timestamp").ToString("HH:mm:ss");
                                TemperatureInsideTextBox.Text = $"{reader.GetDecimal("temperature_inside").ToString("0.#")}°C";
                                TemperatureOutsideTextBox.Text = $"{reader.GetDecimal("temperature_outside").ToString("0.#")}°C";
                                PressureTextBox.Text = $"{reader.GetDecimal("pressure").ToString("0.")} hPa";
                                HumidityTextBox.Text = $"{reader.GetDecimal("humidity").ToString("0.")}%";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd: {ex.Message}", "Błąd połączenia", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

      

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadWeatherData();
        }
    }
}
