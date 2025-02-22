using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace InOculus.Utilities.SmartPause
{
    internal class SmartPauseWatcher
    {
        public event EventHandler<SmartPauseEventArgs> SmartPauseEvent;

        private readonly List<BaseRegistryKeyEventWatcher> registryKeyWatchers = new();

        public SmartPauseWatcher()
        {
            // Cannot use HKEY_CURRENT_USER directly as it's not tracked by `RegistryEvent`, so we need to look through HKEY_USERS.
            var currentUserPath = WindowsIdentity.GetCurrent().User?.Value;

            if (currentUserPath != null)
            {
                // Peripherals - webcam, mic.
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
                            var registryKeyWatcher = new PeripheralUseRegistryKeyEventWatcher(registryKeyPath);
                            registryKeyWatchers.Add(registryKeyWatcher);
                            registryKeyWatcher.SmartPauseEvent += SmartPauseWatcher_SmartPauseEvent;
                        }

                        parentRegistryKey.Close();
                    }
                }

                // Do not disturb mode.
                // TODO.
            }
        }
        private void SmartPauseWatcher_SmartPauseEvent(object sender, SmartPauseEventArgs e)
        {
            SmartPauseEvent(sender, e);
        }

        public void Dispose()
        {
            foreach (var registryKeyWatcher in registryKeyWatchers)
            {
                registryKeyWatcher.Dispose();
            }
        }
    }
}
