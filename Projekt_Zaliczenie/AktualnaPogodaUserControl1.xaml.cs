using System;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;  // Importujemy MySqlConnector

namespace Projekt_Zaliczenie
{
    public partial class AktualnaPogodaUserControl1 : UserControl
    {
        // Zmieniamy connection string na używający MySqlConnector
        private string connectionString = "Server=192.168.1.222;Database=weather_station;Uid=weather_user;Pwd=strong_password;";

        public AktualnaPogodaUserControl1()
        {
            InitializeComponent();
            LoadWeatherData();
        }

        public async void LoadWeatherData()
        {
            string query = "SELECT temperature_inside, temperature_outside, humidity, pressure FROM Last24Hours ORDER BY timestamp DESC LIMIT 1;";

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
                                TemperatureInsideTextBox.Text = $"{reader.GetDecimal("temperature_inside")} °C";
                                TemperatureOutsideTextBox.Text = $"{reader.GetDecimal("temperature_outside")} °C";
                                PressureTextBox.Text = $"{reader.GetDecimal("pressure")} hPa";
                                HumidityTextBox.Text = $"{reader.GetDecimal("humidity")} %";
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
