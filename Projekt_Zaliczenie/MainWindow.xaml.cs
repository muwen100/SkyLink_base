using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Projekt_Zaliczenie
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged

        /*
        Klasa MainWindow dziedziczy po:
           Metrowindow - odpowiadajacy za stylizacje interfejsu
           INotifyPropertyChanged - powiadamiajacy interfejs o zmianach danych
        */

    {
        private string godzina;
        private string data;

        /*
         Godzina reprezentuje aktualną godzinę jako string
        getter zwraca wartość pola godzina 
        setter informuje przez metode OnPropertyChanged UI o zmianie 
         */
        public string Godzina
        {
            get { return godzina; }
            set
            {
                godzina = value;
                OnPropertyChanged(nameof(Godzina));
            }
        }

        //funkcjonalność jak wyżej
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
            DataContext = this; // ustawia kontekst danych na bierzącą klasę

            Timer timer = new Timer(1000); // Tworzy obiekt klasy Timer z okresem 1000ms tj 1 sekunda 
            timer.Elapsed += Timer_Elapsed;
            timer.Start();  //start timera

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) //to jest ta metoda wywolywana przez timer co sekunde 
        {
            Application.Current.Dispatcher.Invoke(() =>      // umożliwia wykonanie kodu w głównym wątku, był problem bo timer działa w osobnym
            {
                DateTime teraz = DateTime.Now;          //pobiera aktualna date i czas
                Godzina = teraz.ToString("HH:mm:ss");   //formatowanie do pożądanej postaci
                Data = teraz.ToString("d");    // jw
                Data = Data + "  ";
            });
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));  //informowanie systemu o zmianie godziny 
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    
    }
}
