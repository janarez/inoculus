using InOculus.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.UI.ViewManagement;

namespace InOculus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntervalTimer IntervalTimer = new IntervalTimer();


        public MainWindow()
        {
            InitializeComponent();
            InitializeColorTheme();
            lblTime.DataContext = IntervalTimer;
        }

        private void InitializeColorTheme()
        {
            var settings = new UISettings();
            var accent = settings.GetColorValue(UIColorType.Accent);
            var background = settings.GetColorValue(UIColorType.Background);

            var accentColor = Color.FromArgb(accent.A, accent.R, accent.G, accent.B);
            var accentBrush = new SolidColorBrush(accentColor);
            var backgroundColor = Color.FromArgb(background.A, background.R, background.G, background.B);
            var backgroundBrush = new SolidColorBrush(backgroundColor);
             
            btnStart.Background = accentBrush;
            btnSettings.Background = accentBrush;
            wndInOculus.Background = backgroundBrush;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            IntervalTimer.Start();
        }
    }
}
