using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Projekt_Zaliczenie
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        private string godzina;
        private string data;

        public string Godzina
        {
            get { return godzina; }
            set
            {
                godzina = value;
                OnPropertyChanged(nameof(Godzina));
            }
        }

        public string Data
        {
            get { return data; }
            set
            {
                data = value;
                OnPropertyChanged(nameof(data));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Timer timer = new Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            MainContent.Content = new AktualnaPogodaUserControl1();
            Header_AktualnaPogoda.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#87cfff"));
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DateTime teraz = DateTime.Now;
                Godzina = teraz.ToString("HH:mm:ss");
                Data = teraz.ToString("d");
                Data = Data + "  ";
            }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AktualnaPogoda_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AktualnaPogodaUserControl1();
            ResetButtonBackgrounds();
            Header_AktualnaPogoda.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#87cfff"));
        }

        private void OsCzasu_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new OsCzasuUserControl1();
            ResetButtonBackgrounds();
            Header_OsCzasu.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#87cfff"));
        }

        private void Wykresy_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new WykresyUserControl1();
            ResetButtonBackgrounds();
            Header_Wykresy.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#87cfff"));
        }

        private void Trendy_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TrendyUserControl1();
            ResetButtonBackgrounds();
            Header_Trendy.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#87cfff"));
        }

        private void Opcje_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new OpcjeUserControl1();
            ResetButtonBackgrounds();
            Header_Opcje.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#87cfff"));
        }

        private void ResetButtonBackgrounds()
        {
            Header_AktualnaPogoda.Background = new SolidColorBrush(Colors.Transparent);
            Header_OsCzasu.Background = new SolidColorBrush(Colors.Transparent);
            Header_Wykresy.Background = new SolidColorBrush(Colors.Transparent);
            Header_Trendy.Background = new SolidColorBrush(Colors.Transparent);
            Header_Opcje.Background = new SolidColorBrush(Colors.Transparent);
        }
    }
}
