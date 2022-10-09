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
        private readonly IntervalTimer IntervalTimer = new IntervalTimer(UserPreferences.FocusInterval);

        public BreakWindow()
        {
            InitializeComponent();
            lblTime.DataContext = IntervalTimer;
            IntervalTimer.Start();
        }

        private void WndBreak_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == (Key)Properties.Settings.Default.BreakWindowCloseKey)
            {
                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            IntervalTimer.Close();
            base.OnClosed(e);
        }
    }
}
