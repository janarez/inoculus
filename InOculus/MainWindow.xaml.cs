using InOculus.Utilities;
using MahApps.Metro.IconPacks.Converter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Data;
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
        private IntervalTimer IntervalTimer;
        private CountDownCircle CountDownCircle;

        private Timer focusTimer;
        private Timer breakTimer;

        private List<BreakWindow> breakWindows;

        private bool focusOn = false;

        private ImageSource imgSrcPlay;
        private ImageSource imgSrcStop;

        public MainWindow()
        {
            // Otherwise `BreakWindow` from the `breakWindows` attribute will be main.
            Application.Current.MainWindow = this;
            ((App)Application.Current).SetState(AppState.Stop);

            InitializeComponent();
            InitializeColorTheme();

            generateCountDownCircleAndFocusTimer();
        }

        private void generateBreakWindowsAndTimer()
        {
#if DEBUG
            var breakInterval = new DisplayedTimeSpan(minutes: 0, seconds: 10);
#else
            var breakInterval = new DisplayedTimeSpan(minutes: 0, seconds: Properties.Settings.Default.BreakInterval);
#endif
            // One break window / monitor.
            breakWindows = new List<BreakWindow>(Screen.AllScreens.Select((Screen s) => new BreakWindow(breakInterval, s.WpfBounds)));
            breakWindows.First().MainOne = true; // First monitor will have keyboard focus.
            breakWindows.ForEach(w => w.Escaped += BreakTimer_Elapsed);

            // Time break also in main window to know when to start another focus round.
            breakTimer = new Timer(breakInterval.TimeSpan.TotalMilliseconds);
            breakTimer.Elapsed += BreakTimer_Elapsed;
        }


        private void generateCountDownCircleAndFocusTimer()
        {
#if DEBUG
            var focusInterval = new DisplayedTimeSpan(minutes: 0, seconds: 5);
#else
            var focusInterval = new DisplayedTimeSpan(minutes: Properties.Settings.Default.FocusInterval, seconds: 0);
#endif
            // Timer to change label with countdown.
            IntervalTimer = new IntervalTimer(focusInterval);
            lblTime.DataContext = IntervalTimer;
            // Countdown circle animation.
            CountDownCircle = new CountDownCircle(focusInterval.TimeSpan.TotalMilliseconds);
            arcCountDown.DataContext = CountDownCircle;

            // Time focus round in main window.
            focusTimer = new Timer(focusInterval.TimeSpan.TotalMilliseconds);
            focusTimer.Elapsed += FocusTimer_Elapsed;
        }

        private void InitializeColorTheme()
        {
            var settings = new UISettings();
            var accent = settings.GetColorValue(UIColorType.Accent);
            var lightAccent = settings.GetColorValue(UIColorType.AccentLight2);
            var background = settings.GetColorValue(UIColorType.Background);

            Color accentColor = Color.FromArgb(accent.A, accent.R, accent.G, accent.B);
            Color lightAccentColor = Color.FromArgb(lightAccent.A, lightAccent.R, lightAccent.G, lightAccent.B);
            Color backgroundColor = Color.FromArgb(200, background.R, background.G, background.B);
            Color grayColor = Color.FromRgb(73, 80, 87); // Bootstrap's v5.2 `gray-700`.

            // Radial brushes for countdown border.
            var gsc = (Color borderColor) => new GradientStopCollection() { new(borderColor, 0.0), new(borderColor, 0.97), new(backgroundColor, 1.0) };
            RadialGradientBrush grayRadialBorder = new RadialGradientBrush(gsc(grayColor));
            RadialGradientBrush lightAccentRadialBorder = new RadialGradientBrush(gsc(lightAccentColor));
            grayRadialBorder.Freeze();
            lightAccentRadialBorder.Freeze();

            Resources.Add("GrayRadialBorderBrush", grayRadialBorder);
            Resources.Add("LightAccentRadialBorderBrush", lightAccentRadialBorder);

            // These are also used in `BreakWindow`.
            var accentBrush = new SolidColorBrush(accentColor);
            Application.Current.Resources.Add("AccentBrush", accentBrush);
            Application.Current.Resources.Add("LightAccentBrush", new SolidColorBrush(lightAccentColor));
            Application.Current.Resources.Add("BackgroundBrush", new SolidColorBrush(backgroundColor));

            // Set color of thumbnail icons.
            IValueConverter converter = new PackIconBootstrapIconsKindToImageConverter { Brush = accentBrush };
            imgSrcPlay = (ImageSource)converter.Convert(Icons.Play, typeof(ImageSource), null, null);
            imgSrcStop = (ImageSource)converter.Convert(Icons.Stop, typeof(ImageSource), null, null);
            thmStart.ImageSource = imgSrcPlay;
        }

        #region CountDownTimer
        private void BtnStart_Click(object sender, EventArgs e)
        {
            // Stop.
            if (focusOn)
            {
                StopFocusing();
                ((App)Application.Current).SetState(AppState.Stop);
                icnPlay.Kind = Icons.Play;
                thmStart.ImageSource = imgSrcPlay;
                arcCountDown.Visibility = Visibility.Hidden;
            }
            // Start.
            else
            {
                StartFocusing();
                ((App)Application.Current).SetState(AppState.Focus);
                icnPlay.Kind = Icons.Stop;
                thmStart.ImageSource = imgSrcStop;
                arcCountDown.Visibility = Visibility.Visible;
            }
        }

        private void FocusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            StopFocusing();
            // Break windows are generated on the go to ensure their reflect current monitor setup.
            Dispatcher.Invoke(() => generateBreakWindowsAndTimer());
            breakTimer.Start();
            Dispatcher.Invoke(() => breakWindows.ForEach(w => w.Show()));
        }

        private void BreakTimer_Elapsed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => breakWindows.ForEach(w => w.Close()));
            breakTimer.Dispose();
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
            Process.Start(new ProcessStartInfo
            {
                FileName = AppPreferences.RepositoryURL,
                UseShellExecute = true
            });
        }

        private void BtnStats_Click(object sender, RoutedEventArgs e)
        {
            return;
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow();
            var hasChanged = Dispatcher.Invoke(() => settings.ShowDialog());

            if (hasChanged == true)
            {
                if (focusOn)
                {
                    // Stop the focus round since we might be changing focus time while its running.
                    BtnStart_Click(null, null);
                }
                // Regenerate break windows and timers with new settings.
                generateBreakWindowsAndTimer();
                generateCountDownCircleAndFocusTimer();
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        public void ToForeground()
        {
            WindowState = WindowState.Normal;
            Activate();
            Focus();
        }

        protected override void OnClosed(EventArgs e)
        {
            IntervalTimer.Dispose();
            CountDownCircle.Dispose();
            focusTimer.Dispose();
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void wndMain_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.StartOnStartup)
            {
                BtnStart_Click(sender, e);
            }
        }
    }
}
