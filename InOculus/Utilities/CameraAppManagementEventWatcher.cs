using Microsoft.Win32;
using System;
using System.Management;

namespace InOculus.Utilities
{
    internal class CameraAppManagementEventWatcher
    {
        public event EventHandler<CameraStateChangedEventArgs> CameraStateChanged;

        private readonly ManagementEventWatcher appLastStopEventWatcher;

        private readonly string appLastStartRegistryValueName = "LastUsedTimeStart";
        private readonly string appLastStopRegistryValueName = "LastUsedTimeStop";

        private readonly RegistryKey appRegistryKey;


        public CameraAppManagementEventWatcher(string registryKeyPath)
        {
            appRegistryKey = Registry.Users.OpenSubKey(registryKeyPath);
            string queryWithoutValue = $"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{registryKeyPath.Replace("\\", "\\\\")}'";

            // Stop watcher.
            appLastStopEventWatcher = new ManagementEventWatcher(new EventQuery(queryWithoutValue + $"AND ValueName = '{appLastStopRegistryValueName}'"));
            appLastStopEventWatcher.EventArrived += appCameraStateChangedEventHandler;
            appLastStopEventWatcher.Start();
        }

        private void appCameraStateChangedEventHandler(object sender, EventArrivedEventArgs e)
        {
            var startTimestampObject = appRegistryKey.GetValue(appLastStartRegistryValueName);
            var startTimestamp = convertActiveDirectoryTimeToDateTime(startTimestampObject);

            var stopTimestampObject = appRegistryKey.GetValue(appLastStopRegistryValueName);
            var stopTimestamp = convertActiveDirectoryTimeToDateTime(stopTimestampObject);

            var appStateToTrigger = (DateTime.Now > startTimestamp && stopTimestamp == null) ? AppState.Pause : AppState.Focus;
            OnCameraStateChanged(new CameraStateChangedEventArgs(appStateToTrigger));
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

        private void OnCameraStateChanged(CameraStateChangedEventArgs e)
        {
            CameraStateChanged(this, e);
        }

        public class CameraStateChangedEventArgs : EventArgs
        {
            public AppState AppStateToTrigger { get; }

            public CameraStateChangedEventArgs(AppState appStateToTrigger)
            {
                AppStateToTrigger = appStateToTrigger;
            }
        }
    }
}
