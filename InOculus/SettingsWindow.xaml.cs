using InOculus.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // Validation settings.
        // ====================
        // Focus interval.
        private const int minimumFocusIntervalMins = 1;
        private const int maximumFocusIntervalMins = 99;
        private readonly Func<bool> isFocusIntervalValid;
        // Break interval.
        private const int minimumBreakIntervalSeconds = 1;
        private const int maximumBreakIntervalSeconds = 99 * 60;
        private readonly Func<bool> isBreakIntervalValid;
        // Break keybord key.
        private readonly HashSet<int> allowedBreakKeyValues = Enum.GetValues<Key>().Select(x => (int)x).ToHashSet();
        private Func<bool> isBreakKeyValid;

        public SettingsWindow()
        {
            InitializeComponent();
            Title = $"{AppPreferences.AppName} Settings";

            // Initialize validators.
            isFocusIntervalValid = () => validateInteger(txbFocus.Text, minAllowed: minimumFocusIntervalMins, maxAllowed: maximumFocusIntervalMins);
            isBreakIntervalValid = () => validateInteger(txbBreak.Text, minAllowed: minimumBreakIntervalSeconds, maxAllowed: maximumBreakIntervalSeconds);
            isBreakKeyValid = () => int.TryParse(txbBreakKey.Text, out int value) && allowedBreakKeyValues.Contains(value);

            // Show current settings.
            txbFocus.Text = Properties.Settings.Default.FocusInterval.ToString();
            txbBreak.Text = Properties.Settings.Default.BreakInterval.ToString();
            txbBreakKey.Text = Properties.Settings.Default.BreakWindowCloseKey.ToString();
            txtBreakKey.Text = ((Key)Properties.Settings.Default.BreakWindowCloseKey).ToString();
            ckbRunOnStartup.IsChecked = Startup.IsRegistered;
            ckbStartOnStartup.IsChecked = Properties.Settings.Default.StartOnStartup;
        }

        private bool validateInteger(string stringValue, int minAllowed, int maxAllowed)
        {
            var isNumber = int.TryParse(stringValue, out int value);
            return isNumber && value >= minAllowed && value <= maxAllowed;
        }

        private void txbFocus_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            txbFocus.Background = isFocusIntervalValid() ? validSettingBrush : invalidSettingBrush;
        }

        private void txbBreak_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            txbBreak.Background = isBreakIntervalValid() ? validSettingBrush : invalidSettingBrush;
        }

        private void txbBreakKey_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var isValid = isBreakKeyValid();
            txbBreakKey.Background = isValid ? validSettingBrush : invalidSettingBrush;
            if (isValid)
            {
                txtBreakKey.Text = ((Key)int.Parse(txbBreakKey.Text)).ToString().ToUpper();
            }
            else
            {
                txtBreakKey.Text = "invalid key value";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var allValid = isFocusIntervalValid() && isBreakIntervalValid() && isBreakKeyValid();
            if (!allValid)
            {
                txtSave.Text = "Could not save invalid settings. Fix all with red background.";
                return;
            }

            // Handle startup registry.
            bool runOnStartUp = (bool)ckbRunOnStartup.IsChecked;
            if (runOnStartUp)
            {
                Startup.Register();
            }
            else
            {
                Startup.Deregister();
            }

            // Save new settings.
            Properties.Settings.Default.FocusInterval = int.Parse(txbFocus.Text);
            Properties.Settings.Default.BreakInterval = int.Parse(txbBreak.Text);
            Properties.Settings.Default.BreakWindowCloseKey = int.Parse(txbBreakKey.Text);
            Properties.Settings.Default.StartOnStartup = (bool)ckbStartOnStartup.IsChecked;
            Properties.Settings.Default.Save();

            DialogResult = true; // To tell main window that settings have changed.
            Close();
        }

        private void wndSettings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSave_Click(sender, e);
            }
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
