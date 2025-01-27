using Microsoft.Win32;
using System;
using System.Management;

namespace InOculus.Utilities
{
    internal class MeetingRegistryKeyManagementEventWatcher
    {
        public event EventHandler<MeetingStateChangedEventArgs> MeetingStateChanged;

        private readonly ManagementEventWatcher appLastStopEventWatcher;

        private readonly string appLastStartRegistryValueName = "LastUsedTimeStart";
        private readonly string appLastStopRegistryValueName = "LastUsedTimeStop";

        private readonly RegistryKey appRegistryKey;


        public MeetingRegistryKeyManagementEventWatcher(string registryKeyPath)
        {
            appRegistryKey = Registry.Users.OpenSubKey(registryKeyPath);
            string queryWithoutValue = $"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{registryKeyPath.Replace("\\", "\\\\")}'";

            // Stop watcher.
            appLastStopEventWatcher = new ManagementEventWatcher(new EventQuery(queryWithoutValue + $"AND ValueName = '{appLastStopRegistryValueName}'"));
            appLastStopEventWatcher.EventArrived += meetingRegistryKeyChangedEventHandler;
            appLastStopEventWatcher.Start();
        }

        private void meetingRegistryKeyChangedEventHandler(object sender, EventArrivedEventArgs e)
        {
            var startTimestampObject = appRegistryKey.GetValue(appLastStartRegistryValueName);
            var startTimestamp = convertActiveDirectoryTimeToDateTime(startTimestampObject);

            var stopTimestampObject = appRegistryKey.GetValue(appLastStopRegistryValueName);
            var stopTimestamp = convertActiveDirectoryTimeToDateTime(stopTimestampObject);

            var appStateToTrigger = (DateTime.Now > startTimestamp && stopTimestamp == null) ? AppState.Pause : AppState.Focus;

            OnMeetingStateChanged(new MeetingStateChangedEventArgs(appStateToTrigger));
        }

        private static DateTime? convertActiveDirectoryTimeToDateTime(object? activeDirectoryTime)
        {
            if (activeDirectoryTime == null || (long)activeDirectoryTime == 0)
            {
                return null;
            }

            return DateTime.FromFileTimeUtc((long)activeDirectoryTime);
        }

        public void Dispose()
        {
            appRegistryKey.Close();
            appLastStopEventWatcher.Stop();
            appLastStopEventWatcher.Dispose();
        }

        private void OnMeetingStateChanged(MeetingStateChangedEventArgs e)
        {
            MeetingStateChanged(this, e);
        }
    }
}
