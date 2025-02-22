using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace InOculus.Utilities.SmartPause
{
    internal class SmartPauseManager
    {
        public event EventHandler<SmartPauseEventArgs> SmartPauseStateChanged;

        private readonly List<ISmartPauseWatcher> watchers = new();

        public SmartPauseManager()
        {
            var doNotDisturbPoller = new DoNotDisturbModePoller();
            doNotDisturbPoller.SmartPauseStateChanged += SmartPauseWatcher_SmartPauseStateChanged;

            // Cannot use HKEY_CURRENT_USER directly as it's not tracked by `RegistryEvent`, so we need to look through HKEY_USERS.
            var currentUserPath = WindowsIdentity.GetCurrent().User?.Value;

            if (currentUserPath != null)
            {
                // Webcam, mic.
                var consentStorePath = $"{currentUserPath}\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore";
                var consentsToMonitor = new List<string> {
                    "webcam", // System (camera app, Teams) apps with camera access.
                    "webcam\\NonPackaged", // Installed (Zoom, browser) apps with camera access.
                    "microphone",
                    "microphone\\NonPackaged",
                };

                foreach (var consent in consentsToMonitor)
                {
                    var constentPath = $"{consentStorePath}\\{consent}";
                    var parentRegistryKey = Registry.Users.OpenSubKey(constentPath);

                    if (parentRegistryKey != null)
                    {
                        foreach (var appName in parentRegistryKey.GetSubKeyNames())
                        {
                            var registryKeyPath = $"{constentPath}\\{appName}";
                            var registryKeyWatcher = new LastUsedTimeRegistryKeyWatcher(registryKeyPath);
                            watchers.Add(registryKeyWatcher);
                            registryKeyWatcher.SmartPauseStateChanged += SmartPauseWatcher_SmartPauseStateChanged;
                        }

                        parentRegistryKey.Close();
                    }
                }
            }
        }
        private void SmartPauseWatcher_SmartPauseStateChanged(object sender, EventArgs e)
        {
            foreach (var watcher in watchers)
            {
                var state = watcher.GetSmartPauseState();
                if (state == SmartPauseState.On)
                {
                    SmartPauseStateChanged?.Invoke(sender, new SmartPauseEventArgs(SmartPauseState.On));
                    return;
                }
            }
            SmartPauseStateChanged?.Invoke(sender, new SmartPauseEventArgs(SmartPauseState.Off));
        }

        public void Dispose()
        {
            foreach (var watcher in watchers)
            {
                watcher.Dispose();
            }
        }
    }
}
