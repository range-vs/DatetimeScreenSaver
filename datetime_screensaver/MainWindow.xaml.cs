using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Threading;

namespace datetime_screensaver
{
    public partial class MainWindow : Window
    {
        public DispatcherTimer Timer { get; set; }
        public bool IsTimeSplitter { get; set; } = true;
        public DispatcherTimer TimerSplitter { get; set; }
        private DateTime LastSystemTime { get; set; }
        public DispatcherTimer TimerChangeColor { get; set; }
        public bool ModeColor { get; set; } = true;

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
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

        private void ChangeColor(object sender, EventArgs e)
        {
            ModeColor = !ModeColor;
            if (ModeColor)
            {
                Date.Foreground = new SolidColorBrush(Colors.White);
                Time.Foreground = new SolidColorBrush(Colors.White);
                Window.Background = new SolidColorBrush(Colors.Black);
            }
            else
            {
                Date.Foreground = new SolidColorBrush(Colors.Black);
                Time.Foreground = new SolidColorBrush(Colors.Black);
                Window.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0D4054"));
            }
        }
    }
}
