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
   
    public partial class OsCzasuUserControl1 : UserControl
    {
        private string connectionString = "Server=raspberrypi;Database=weather_station;Uid=weather_user;Pwd=strong_password;";
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

            DateTime selectedDate = datownik.SelectedDate.Value; //wybrana data z kalendarza
            string query = "SELECT avg_temp_inside, avg_temp_outside, avg_humidity, avg_pressure FROM Last31Days WHERE DATE(timestamp) = @selectedDate ORDER BY timestamp DESC LIMIT 1;"; // Zapytanie SQL

            using (var connection = new MySqlConnection(connectionString)) //using na początk, żeby połączenie było zamykane po wyjściu
            {
                try
                {
                    await connection.OpenAsync(); // łączenie asynchroniczne, żeby nie blokować interfejsu
                    //MessageBox.Show("Połączenie z bazą danych powiodło się!");

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@selectedDate", selectedDate.ToString("yyyy-MM-dd"));

                        using (var reader = await command.ExecuteReaderAsync()) // odczyt asynchroniczny
                        {
                            if (await reader.ReadAsync()) 
                            {
                                TemperatureIAvgTextBox.Text = $"{reader.GetDecimal("avg_temp_inside").ToString("0.0")}°C"; // odczytanie wartości deimal i zapisanie do stringa z jednym miejscem po przecinku
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
