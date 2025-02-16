using Microsoft.Win32;
using System;
using System.Management;

namespace InOculus.Utilities
{
    internal abstract class BaseRegistryKeyEventWatcher
    {
        public event EventHandler<MeetingStateChangedEventArgs> MeetingStateChanged;

        protected readonly ManagementEventWatcher eventWatcher;
        protected readonly RegistryKey registryKey;

        protected BaseRegistryKeyEventWatcher(string registryKeyPath, string registryValueName)
        {
            registryKey = Registry.Users.OpenSubKey(registryKeyPath);
            string queryWithoutValue = $"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{registryKeyPath.Replace("\\", "\\\\")}'";

            // Stop watcher.
            eventWatcher = new ManagementEventWatcher(new EventQuery(queryWithoutValue + $"AND ValueName = '{registryValueName}'"));
            eventWatcher.EventArrived += registryKeyChangedEventHandler;
            eventWatcher.Start();
        }

        protected abstract void registryKeyChangedEventHandler(object sender, EventArrivedEventArgs e);


        public void Dispose()
        {
            registryKey.Close();
            eventWatcher.Stop();
            eventWatcher.Dispose();
        }

        protected void OnMeetingStateChanged(MeetingStateChangedEventArgs e)
        {
            MeetingStateChanged?.Invoke(this, e);
        }
    }
}
