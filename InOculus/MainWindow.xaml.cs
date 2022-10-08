using InOculus.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.UI.ViewManagement;
using System.Timers;
using System.ComponentModel;
using System;

namespace InOculus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IntervalTimer IntervalTimer = new IntervalTimer();
        private readonly CountDownCircle CountDownCircle = new CountDownCircle();
        
        private readonly Timer focusTimer = new Timer(UserPreferences.FocusInterval.TotalMilliseconds);
        private readonly Timer breakTimer = new Timer(UserPreferences.BreakInterval.TotalMilliseconds);

        private bool focusOn = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeColorTheme();

            lblTime.DataContext = IntervalTimer;
            arcCountDown.DataContext = CountDownCircle;

            focusTimer.Elapsed += FocusTimer_Elapsed; 
            breakTimer.Elapsed += BreakTimer_Elapsed;
        }

        private void InitializeColorTheme()
        {
            var settings = new UISettings();
            var accent = settings.GetColorValue(UIColorType.Accent);
            var background = settings.GetColorValue(UIColorType.Background);

            var accentColor = Color.FromArgb(accent.A, accent.R, accent.G, accent.B);
            var accentBrush = new SolidColorBrush(accentColor);
            // TODO: Alpha not working?
            var backgroundColor = Color.FromArgb(125, background.R, background.G, background.B);
            var backgroundBrush = new SolidColorBrush(backgroundColor);

            btnStart.Background = accentBrush;
            btnSettings.Foreground = accentBrush;
            btnStats.Foreground = accentBrush;
            wndMain.Background = backgroundBrush;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {   
            // Stop.
            if (focusOn)
            {
                StopFocusing();
                icnPlay.Kind = Icons.Play;
                arcCountDown.Visibility = Visibility.Hidden;
            }
            // Start.
            else
            {
                StartFocusing();
                icnPlay.Kind = Icons.Stop;
                arcCountDown.Visibility = Visibility.Visible;
            }
        }

        private void FocusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            StopFocusing();
            breakTimer.Start();
        }

        private void BreakTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            breakTimer.Stop();
            StartFocusing();
        }

        private void StopFocusing()
        {
            focusTimer.Stop();
            IntervalTimer.Stop();
            CountDownCircle.Stop();
            focusOn = false;
        }

        private void StartFocusing()
        {
            focusTimer.Start();
            IntervalTimer.Start();
            CountDownCircle.Start();
            focusOn = true;
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            var breakWindow = new BreakWindow();
            breakWindow.Show();
        }
    }
}
