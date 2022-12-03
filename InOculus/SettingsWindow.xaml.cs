using InOculus.Utilities;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace InOculus
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Brush invalidSettingBrush = new SolidColorBrush(Color.FromRgb(234, 134, 143)); // Bootstrap's v5.2 `red-300`.
        private Brush validSettingBrush = new SolidColorBrush(Color.FromRgb(163, 207, 187)); // Bootstrap's v5.2 `green-200`.


        public SettingsWindow()
        {
            InitializeComponent();
            Title = $"{AppPreferences.AppName} Settings";

            // Show current settings.
            txbFocus.Text = Properties.Settings.Default.FocusInterval.ToString();
            txbFocus.Background = validSettingBrush;
            txbBreak.Text = Properties.Settings.Default.BreakInterval.ToString();
            txbBreak.Background = validSettingBrush;
            txbBreakKey.Text = Properties.Settings.Default.BreakWindowCloseKey.ToString();
            txbBreakKey.Background = validSettingBrush;
            txtBreakKey.Text = ((Key)Properties.Settings.Default.BreakWindowCloseKey).ToString();
            ckbStartup.IsChecked = Properties.Settings.Default.RunOnStartup;
            ckbStartup.Background = validSettingBrush;
        }

        private bool validateInteger(string stringValue, int minAllowed, int maxAllowed)
        {
            var isNumber = int.TryParse(stringValue, out int value);
            return isNumber && value >= minAllowed && value <= maxAllowed;
        }

        private void txbFocus_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            txbFocus.Background = validateInteger(txbFocus.Text, minAllowed: 1, maxAllowed: 99) ? validSettingBrush : invalidSettingBrush;
        }

        private void txbBreak_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            txbBreak.Background = validateInteger(txbBreak.Text, minAllowed: 1, maxAllowed: 99 * 60) ? validSettingBrush : invalidSettingBrush;
        }

        private void txbBreakKey_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var isValid = validateInteger(txbBreakKey.Text, minAllowed: 1, maxAllowed: 172);
            txbBreakKey.Background = isValid ? validSettingBrush : invalidSettingBrush;
            if (isValid)
            {
                txtBreakKey.Text = ((Key)int.Parse(txbBreakKey.Text)).ToString();
            }
            else
            {
                txtBreakKey.Text = "invalid key value";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var allValid = false;
            if (allValid)
            {
                Properties.Settings.Default.FocusInterval = int.Parse(txbFocus.Text);
                Properties.Settings.Default.BreakInterval = int.Parse(txbBreak.Text);
                Properties.Settings.Default.BreakWindowCloseKey = int.Parse(txbBreakKey.Text);
                Properties.Settings.Default.RunOnStartup = (bool)ckbStartup.IsChecked;
            }
            else
            {
                txtSave.Text = "Could not save invalid settings. Fix all with red background.";
            }
        }
    }
}
