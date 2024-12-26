using LiveCharts;
using LiveCharts.Wpf;
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
    /// Logika interakcji dla klasy WykresyUserControl1.xaml
    /// </summary>
    public partial class WykresyUserControl1 : UserControl
    {
        private string connectionString = "Server=192.168.1.222;Database=weather_station;Uid=weather_user;Pwd=strong_password;";
        private Dictionary<string, (string ColumnName, string DisplayName, string Unit)> categoryMap = new Dictionary<string, (string, string, string)>
            {
                { "Temperatura wewnętrzna", ("temperature_inside", "Temperatura wewnętrzna", "°C") },
                { "Temperatura zewnętrzna", ("temperature_outside", "Temperatura zewnętrzna", "°C") },
                { "Wilgotność", ("humidity", "Wilgotność", "%") },
                { "Ciśnienie", ("pressure", "Ciśnienie", "hPa") }
            };

        public WykresyUserControl1()
        {
            InitializeComponent();
            CategoryComboBox.SelectedIndex = 1; // Ustawienie domyślnie wybranej pozycji
            LoadWeatherChartData("temperature_outside", "Temperatura zewnętrzna", "°C"); // Domyślna kategoria
        }

        public async void LoadWeatherChartData(string columnName, string displayName, string unit)
        {
            DateTime now = DateTime.Now;
            DateTime startTime = now.AddHours(-24);
            string query = $@"
                    SELECT 
                        DATE_FORMAT(timestamp, '%Y-%m-%d %H:00:00') AS time_group, 
                        AVG({columnName}) AS avg_value 
                    FROM 
                        Last24Hours 
                    WHERE 
                        timestamp >= @startTime AND timestamp <= @endTime 
                    GROUP BY 
                        time_group 
                    ORDER BY 
                        time_group;";

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@startTime", startTime);
                        command.Parameters.AddWithValue("@endTime", now);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var values = new ChartValues<double>();
                            var labels = new List<string>();

                            while (await reader.ReadAsync())
                            {
                                values.Add(Math.Round(reader.GetDouble("avg_value"), 2));
                                labels.Add(reader.GetString("time_group"));
                            }

                            WeatherChart.Series = new SeriesCollection
                                {
                                    new LineSeries
                                    {
                                        Title = displayName,
                                        Values = values
                                    }
                                };

                            WeatherChart.AxisX.Clear();
                            WeatherChart.AxisX.Add(new Axis
                            {
                                Title = "Czas",
                                Labels = labels
                            });

                            WeatherChart.AxisY.Clear();
                            WeatherChart.AxisY.Add(new Axis
                            {
                                Title = $"{displayName} ({unit})"
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd: {ex.Message}", "Błąd połączenia", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string category = selectedItem.Content.ToString();
                if (categoryMap.TryGetValue(category, out var categoryInfo))
                {
                    LoadWeatherChartData(categoryInfo.ColumnName, categoryInfo.DisplayName, categoryInfo.Unit);
                }
            }
        }
    }
}
