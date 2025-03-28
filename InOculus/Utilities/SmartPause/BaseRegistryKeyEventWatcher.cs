﻿using Microsoft.Win32;
using System.Management;

namespace InOculus.Utilities.SmartPause
{
    internal abstract class BaseRegistryKeyEventWatcher
    {
        protected readonly ManagementEventWatcher eventWatcher;
        protected readonly RegistryKey registryKey;

        protected BaseRegistryKeyEventWatcher(string registryKeyPath, string registryValueName)
        {
            registryKey = Registry.Users.OpenSubKey(registryKeyPath);
            string queryWithoutValue = $"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{registryKeyPath.Replace("\\", "\\\\")}'";

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
    }
}
