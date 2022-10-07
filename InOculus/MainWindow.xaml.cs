using InOculus.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.UI.ViewManagement;

namespace InOculus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IntervalTimer IntervalTimer = new IntervalTimer();

        public MainWindow()
        {
            InitializeComponent();
            InitializeColorTheme();
            lblTime.DataContext = IntervalTimer;
            arcCountDown.DataContext = IntervalTimer.CountDownCircle;
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
            wndInOculus.Background = backgroundBrush;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (IntervalTimer.IsOn)
            {
                IntervalTimer.Reset();
                icnPlay.Kind = Icons.Play;
            }
            else
            {
                IntervalTimer.Start();
                icnPlay.Kind = Icons.Stop;
            }

        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            var n = new Button();
            n.Content = "Janicka";

            btnSettings.Content = n;
        }
    }
}
