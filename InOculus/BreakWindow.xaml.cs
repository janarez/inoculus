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

        public event EventHandler Escaped;

        public BreakWindow(DisplayedTimeSpan breakInterval, Rect wpfBounds)
        {
            InitializeComponent();
            intervalTimer = new IntervalTimer(breakInterval);

            Top = wpfBounds.Top;
            Left = wpfBounds.Left;
            Width = wpfBounds.Width;
            Height = wpfBounds.Height;
            lblTime.DataContext = intervalTimer;
            txtInstructions.Text = $"Press {((Key)Properties.Settings.Default.BreakWindowCloseKey).ToString().ToUpper()} to start over";
        }

        /// <summary>
        /// Whether this is the main of break windows.
        /// </summary>
        public bool MainOne { get; set; }

        private void WndBreak_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == (Key)Properties.Settings.Default.BreakWindowCloseKey)
            {
                Escaped(sender, e);
            }
        }

        public void StopAndHide()
        {
            intervalTimer.Stop();
            Hide();
        }

        public void StartAndShow()
        {
            intervalTimer.Start();
            Show();
            Activate();

            // Force focus the main window.
            if (MainOne)
            {
#if DEBUG
                Focus();
#else
                Interop.FocusWindow(new System.Windows.Interop.WindowInteropHelper(this).EnsureHandle());
#endif
            }
        }

        private void WndBreak_Loaded(object sender, RoutedEventArgs e)
        {
            // Necessary to call after window is loaded, otherwise it is maximized on primary monitor.
            WindowState = WindowState.Maximized;
        }
    }
}
