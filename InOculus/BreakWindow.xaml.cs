using InOculus.Properties;
using InOculus.Utilities;
using System;
using System.Windows;
using System.Windows.Input;

namespace InOculus
{
    /// <summary>
    /// Interaction logic for BreakWindow.xaml
    /// </summary>
    public partial class BreakWindow : Window
    {
        private readonly IntervalTimer intervalTimer;

        public event EventHandler<int?> Escaped;
        public event EventHandler Stopped;

        public BreakWindow(DisplayedTimeSpan breakInterval, Rect wpfBounds)
        {
            InitializeComponent();
            intervalTimer = new IntervalTimer(breakInterval);

            Top = wpfBounds.Top;
            Left = wpfBounds.Left;
            Width = wpfBounds.Width;
            Height = wpfBounds.Height;
            lblTime.DataContext = intervalTimer;
            txtInstructions.Text = $"Press {((Key)Settings.Default.BreakWindowCloseKey).ToString().ToUpper()} to start over";
        }

        /// <summary>
        /// Whether this is the main of break windows.
        /// </summary>
        public bool MainOne { get; set; }

        private void WndBreak_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D0)
            {
                Stopped(sender, e);
                e.Handled = true;
            }

            foreach (var (key, focusInterval) in new[] {
                ((Key)Settings.Default.BreakWindowCloseKey, (int?)null),
                (Key.D1, 1), (Key.D2, 2), (Key.D3, 3), (Key.D4, 4), (Key.D5, 5), (Key.D6, 6), (Key.D7, 7), (Key.D8, 8), (Key.D9, 9)
            })
            {
                if (e.Key == key)
                {
                    Escaped(sender, focusInterval);
                    e.Handled = true;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            intervalTimer.Dispose();
            base.OnClosed(e);
        }


        private void WndBreak_Loaded(object sender, RoutedEventArgs e)
        {
            // Necessary to call after window is loaded, otherwise it is maximized on primary monitor.
            WindowState = WindowState.Maximized;
            intervalTimer.Start();
            Activate();

            // Force focus the main window.
            if (MainOne)
            {
#if DEBUG
                Focus();
#else
                FocusInterop.FocusWindow(this.GetWindowHandle());
#endif
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Escaped(sender, null);
        }
    }
}
