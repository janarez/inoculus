using InOculus.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InOculus
{
    /// <summary>
    /// Interaction logic for BreakWindow.xaml
    /// </summary>
    public partial class BreakWindow : Window
    {
        private readonly IntervalTimer intervalTimer = new IntervalTimer(UserPreferences.FocusInterval);

        public event EventHandler Escaped;

        public BreakWindow()
        {
            InitializeComponent();
            lblTime.DataContext = intervalTimer;
        }

        private void WndBreak_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == (Key)Properties.Settings.Default.BreakWindowCloseKey)
            {
                Escaped(sender, e);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            intervalTimer.Close();
            base.OnClosed(e);
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
        }
    }
}
