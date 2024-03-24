using InOculus.Properties;
using InOculus.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        // Keybord key.
        private readonly HashSet<int> allowedKeyboardKeyValues = Enum.GetValues<Key>().Select(x => (int)x).ToHashSet();
        private Func<string, bool> isKeyboardKeyValid;

        public SettingsWindow()
        {
            InitializeComponent();
            Title = $"{AppPreferences.AppName} Settings";

            // Initialize validators.
            isFocusIntervalValid = () => validateInteger(txbFocus.Text, minAllowed: minimumFocusIntervalMins, maxAllowed: maximumFocusIntervalMins);
            isBreakIntervalValid = () => validateInteger(txbBreak.Text, minAllowed: minimumBreakIntervalSeconds, maxAllowed: maximumBreakIntervalSeconds);
            isKeyboardKeyValid = (string keyboardKey) => int.TryParse(keyboardKey, out int value) && allowedKeyboardKeyValues.Contains(value);

            // Show current settings.
            txbFocus.Text = Settings.Default.FocusInterval.ToString();
            txbBreak.Text = Settings.Default.BreakInterval.ToString();

            txtboxBreakWindowBreakKey.Text = Settings.Default.BreakWindowCloseKey.ToString();
            txtblockBreakWindowBreakKey.Text = ((Key)Settings.Default.BreakWindowCloseKey).ToString();

            txtboxBreakWindowStopAppKey.Text = Settings.Default.BreakWindowStopAppKey.ToString();
            txtblockBreakWindowStopAppKey.Text = ((Key)Settings.Default.BreakWindowStopAppKey).ToString();

            ckbRunOnStartup.IsChecked = Startup.IsRegistered;
            ckbStartOnStartup.IsChecked = Settings.Default.StartOnStartup;
            ckbStartMinimized.IsChecked = Settings.Default.StartMinimized;
        }

        private bool validateInteger(string stringValue, int minAllowed, int maxAllowed)
        {
            var isNumber = int.TryParse(stringValue, out int value);
            return isNumber && value >= minAllowed && value <= maxAllowed;
        }

        private void txbFocus_TextChanged(object sender, TextChangedEventArgs e)
        {
            txbFocus.Background = isFocusIntervalValid() ? validSettingBrush : invalidSettingBrush;
        }

        private void txbBreak_TextChanged(object sender, TextChangedEventArgs e)
        {
            txbBreak.Background = isBreakIntervalValid() ? validSettingBrush : invalidSettingBrush;
        }

        private void txtboxBreakWindowBreakKey_TextChanged(object sender, TextChangedEventArgs e)
        {
            validateSelectedKeyboardKey(txtboxBreakWindowBreakKey, txtblockBreakWindowBreakKey);
        }

        private void txtboxBreakWindowStopAppKey_TextChanged(object sender, TextChangedEventArgs e)
        {
            validateSelectedKeyboardKey(txtboxBreakWindowStopAppKey, txtblockBreakWindowStopAppKey);
        }



        private void validateSelectedKeyboardKey(TextBox textBox, TextBlock textBlock)
        {
            var isValid = isKeyboardKeyValid(textBox.Text);
            textBox.Background = isValid ? validSettingBrush : invalidSettingBrush;
            if (isValid)
            {
                textBlock.Text = ((Key)int.Parse(textBox.Text)).ToString().ToUpper();
            }
            else
            {
                textBlock.Text = "invalid key value";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var allValid = (
                isFocusIntervalValid()
                && isBreakIntervalValid()
                && isKeyboardKeyValid(txtboxBreakWindowBreakKey.Text)
                && isKeyboardKeyValid(txtboxBreakWindowStopAppKey.Text)
            );
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
            Settings.Default.FocusInterval = int.Parse(txbFocus.Text);
            Settings.Default.BreakInterval = int.Parse(txbBreak.Text);

            Settings.Default.BreakWindowCloseKey = int.Parse(txtboxBreakWindowBreakKey.Text);
            Settings.Default.BreakWindowStopAppKey = int.Parse(txtboxBreakWindowStopAppKey.Text);

            Settings.Default.StartOnStartup = (bool)ckbStartOnStartup.IsChecked;
            Settings.Default.StartMinimized = (bool)ckbStartMinimized.IsChecked;

            Settings.Default.Save();

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
