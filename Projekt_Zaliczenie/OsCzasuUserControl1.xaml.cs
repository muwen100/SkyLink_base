using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projekt_Zaliczenie
{
    /// <summary>
    /// Logika interakcji dla klasy OsCzasuUserControl1.xaml
    /// </summary>
    public partial class OsCzasuUserControl1 : UserControl
    {
        private string connectionString = "Server=192.168.1.222;Database=weather_station;Uid=weather_user;Pwd=strong_password;";
        public OsCzasuUserControl1()
        {
            InitializeComponent();
            datownik.SelectedDate = DateTime.Now.AddDays(-1); // Ustawienie domyślnej daty na wczoraj
            Button_Click(null, null);
        }

        public async void LoadAvgWeatherData()
        {
            if (datownik.SelectedDate == null)
            {
                MessageBox.Show("Proszę wybrać datę.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime selectedDate = datownik.SelectedDate.Value;
            string query = "SELECT avg_temp_inside, avg_temp_outside, avg_humidity, avg_pressure FROM Last31Days WHERE DATE(timestamp) = @selectedDate ORDER BY timestamp DESC LIMIT 1;";

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync(); // Używamy asynchronicznego otwarcia połączenia
                    //MessageBox.Show("Połączenie z bazą danych powiodło się!");

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@selectedDate", selectedDate.ToString("yyyy-MM-dd"));

                        using (var reader = await command.ExecuteReaderAsync()) // Używamy asynchronicznego odczytu
                        {
                            if (await reader.ReadAsync()) // Asynchroniczny sposób odczytu wiersza
                            {
                                TemperatureIAvgTextBox.Text = $"{reader.GetDecimal("avg_temp_inside").ToString("0.0")}°C";
                                TemperatureOAvgTextBox.Text = $"{reader.GetDecimal("avg_temp_outside").ToString("0.0")}°C";
                                PressureAvgTextBox.Text = $"{reader.GetDecimal("avg_pressure").ToString("0.0")} hPa";
                                HumidityAvgTextBox.Text = $"{reader.GetDecimal("avg_humidity").ToString("0.0")}%";
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadAvgWeatherData();
        }



    }
}
