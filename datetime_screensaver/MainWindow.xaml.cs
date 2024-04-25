using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
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
using System.Threading;
using System.Windows.Threading;
using System.IO;

namespace datetime_screensaver
{
    public partial class MainWindow : Window
    {
        private static string FileName = "appconfig.json";
        private static string WeatherApiKey = "";
        private static string CityKey = "";

        public DispatcherTimer Timer { get; set; }
        public bool IsTimeSplitter { get; set; } = true;
        public DispatcherTimer TimerSplitter { get; set; }
        private DateTime LastSystemTime { get; set; }
        public DispatcherTimer TimerChangeColor { get; set; }
        public bool ModeColor { get; set; } = true;
        public DispatcherTimer WeatherTimer { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            try
            {
                using (StreamReader sr = new StreamReader(FileName))
                {
                    string fileContent = sr.ReadToEnd();
                    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(fileContent);
                    WeatherApiKey = data.WEATHER_API_KEY;
                    CityKey = data.CITY_KEY;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка получения ключей погоды");
            }

            Mouse.OverrideCursor = Cursors.None;

            WindowState = WindowState.Maximized; // Установка окна в состояние "Максимизировано"
            WindowStyle = WindowStyle.None; // Убирает рамку окна

            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(UpdateTime);
            Timer.Interval = TimeSpan.FromMilliseconds(1000);
            Timer.Start();

            TimerChangeColor = new DispatcherTimer();
            TimerChangeColor.Tick += new EventHandler(ChangeColor);
            TimerChangeColor.Interval = TimeSpan.FromMinutes(20);
            TimerChangeColor.Start();

            WeatherTimer = new DispatcherTimer();
            WeatherTimer.Tick += new EventHandler(UpdateWeather);
            WeatherTimer.Interval = TimeSpan.FromMinutes(5);
            WeatherTimer.Start();
            UpdateWeather(null, null);

            KeyDown += KeyDownHandler;
            MouseWheel += MouseWheelHandler;
            //MouseMove += MouseMoveHandler;
            MouseLeftButtonDown += MouseDownHandler;
            MouseRightButtonDown += MouseDownHandler;

            //TimerSplitter = new DispatcherTimer();
            //TimerSplitter.Tick += new EventHandler(UpdateSplitter);
            //TimerSplitter.Interval = TimeSpan.FromMilliseconds(1000);
            //TimerSplitter.Start();
        }

        //private void MouseMoveHandler(object sender, MouseEventArgs e)
        //{
        //    Application.Current.Shutdown();
        //}

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void UpdateTime(object sender, EventArgs e)
        {
            // Коррекция интервала таймера
            DateTime currentSystemTime = DateTime.Now;
            TimeSpan timeDifference = currentSystemTime - LastSystemTime;
            LastSystemTime = currentSystemTime;
            if (Timer.Interval.Subtract(timeDifference) > TimeSpan.Zero)
            {
                Timer.Interval = Timer.Interval.Subtract(timeDifference);
            }

            DateTime now = DateTime.Now;
            CultureInfo culture = new CultureInfo("ru-RU");
            var date = now.ToString("dddd, dd MMMM yyyy", culture);
            date = date[0].ToString().ToUpper() + date.Substring(1);
            Date.Text = date;
            var time = DateTime.Now.ToString("HH:mm:ss");
            Time.Text = time; // IsTimeSplitter ? time : time.Replace(":", " ");
        }

        private void UpdateSplitter(object sender, EventArgs e)
        {
            //IsTimeSplitter = !IsTimeSplitter;
        }

        private void UpdateWeather(object sender, EventArgs e)
        {
            string apiKey = "1e93f3de28da4dec987231925242204";
            string city = "Воронеж";
            string url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={city}&lang=ru";//$"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&lang=ru";

            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);

                    string weatherDescription = data.current.condition.text;
                    double temperature = data.current.temp_c;
                    int humidity = data.current.humidity;

                    string _out = string.Format($"Погода в {city}: {weatherDescription}\nТемпература: {temperature}°C\nВлажность: {humidity}%\n");
                    Console.WriteLine(_out);
                    Weather.Text = _out;
                }
                else
                {
                    string _out = $"Ошибка получения погоды: {response.ReasonPhrase}";
                    Console.WriteLine(_out);
                    Weather.Text = _out;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка получения погоды");
            }
        }

        private void ChangeColor(object sender, EventArgs e)
        {
            ModeColor = !ModeColor;
            if (ModeColor)
            {
                Date.Foreground = new SolidColorBrush(Colors.White);
                Time.Foreground = new SolidColorBrush(Colors.White);
                Weather.Foreground = new SolidColorBrush(Colors.White);
                Window.Background = new SolidColorBrush(Colors.Black);
            }
            else
            {
                Date.Foreground = new SolidColorBrush(Colors.Gold);
                Time.Foreground = new SolidColorBrush(Colors.Gold);
                Weather.Foreground = new SolidColorBrush(Colors.Gold);
                Window.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0D4054"));
            }
        }
    }
}
