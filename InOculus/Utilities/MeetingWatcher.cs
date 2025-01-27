using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace InOculus.Utilities
{
    internal class MeetingWatcher
    {
        public event EventHandler<MeetingStateChangedEventArgs> MeetingStateChanged;

        private readonly List<MeetingRegistryKeyManagementEventWatcher> meetingEventWatchers = new();

        public MeetingWatcher()
        {
            // Cannot use HKEY_CURRENT_USER directly as it's not tracked by `RegistryEvent`, so we need to look through HKEY_USERS.
            var currentUserPath = WindowsIdentity.GetCurrent().User?.Value;

            if (currentUserPath != null)
            {
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
                            var meetingWatcher = new MeetingRegistryKeyManagementEventWatcher(registryKeyPath);
                            meetingEventWatchers.Add(meetingWatcher);
                            meetingWatcher.MeetingStateChanged += MeetingWatcher_MeetingStateChanged;
                        }

                        parentRegistryKey.Close();
                    }
                }
            }
        }
        private void MeetingWatcher_MeetingStateChanged(object sender, MeetingStateChangedEventArgs e)
        {
            MeetingStateChanged(sender, e);
        }

        public void Dispose()
        {
            foreach (var meetingEventWatcher in meetingEventWatchers)
            {
                meetingEventWatcher.Dispose();
            }
        }
    }
}
