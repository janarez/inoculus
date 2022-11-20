using InOculus.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Windows.UI.ViewManagement;
using WpfScreenHelper;

namespace InOculus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IntervalTimer IntervalTimer = new IntervalTimer(UserPreferences.FocusInterval);
        private readonly CountDownCircle CountDownCircle = new CountDownCircle();
        // One break window / monitor.
        private readonly List<BreakWindow> breakWindows = new List<BreakWindow>(Screen.AllScreens.Select((Screen s) => new BreakWindow(s.WpfBounds)));

        private readonly Timer focusTimer = new Timer(UserPreferences.FocusInterval.TimeSpan.TotalMilliseconds);
        private readonly Timer breakTimer = new Timer(UserPreferences.BreakInterval.TimeSpan.TotalMilliseconds);

        private bool focusOn = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeColorTheme();

            lblTime.DataContext = IntervalTimer;
            arcCountDown.DataContext = CountDownCircle;

            focusTimer.Elapsed += FocusTimer_Elapsed;
            breakTimer.Elapsed += BreakTimer_Elapsed;
            breakWindows.ForEach(w => w.Escaped += BreakTimer_Elapsed);
        }

        private void InitializeColorTheme()
        {
            var settings = new UISettings();
            var accent = settings.GetColorValue(UIColorType.Accent);
            var lightAccent = settings.GetColorValue(UIColorType.AccentLight2);
            var background = settings.GetColorValue(UIColorType.Background);

            Resources.Add("AccentBrush", new SolidColorBrush(Color.FromArgb(accent.A, accent.R, accent.G, accent.B)));
            Resources.Add("LightAccentBrush", new SolidColorBrush(Color.FromArgb(lightAccent.A, lightAccent.R, lightAccent.G, lightAccent.B)));
            Background = new SolidColorBrush(Color.FromArgb(200, background.R, background.G, background.B));
        }

        #region CountDownTimer
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
            Dispatcher.Invoke(() => breakWindows.ForEach(w => w.StartAndShow()));
        }

        private void BreakTimer_Elapsed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => breakWindows.ForEach(w => w.StopAndHide()));
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
        #endregion

        #region TopBarButtons
        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            return;
        }

        private void BtnStats_Click(object sender, RoutedEventArgs e)
        {
            return;
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            return;
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        protected override void OnClosed(EventArgs e)
        {
            IntervalTimer.Close();
            CountDownCircle.Close();
            focusTimer.Close();
            breakTimer.Close();
            base.OnClosed(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
