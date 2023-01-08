using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;

namespace InOculus.Utilities
{
    static class Startup
    {
        private const string registryName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private static readonly string assemblyLocation = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Programs), AppPreferences.AppName, $"{AppPreferences.AppName}.appref-ms");


        public static void Register()
        {
            using var key = Registry.CurrentUser.OpenSubKey(registryName, true);
            if (!key.GetValueNames().Contains(AppPreferences.AppName))
            {
                key.SetValue(AppPreferences.AppName, assemblyLocation, RegistryValueKind.String);
            }
        }

        public static void Deregister()
        {
            using var key = Registry.CurrentUser.OpenSubKey(registryName, true);
            if (key.GetValueNames().Contains(AppPreferences.AppName))
            {
                key.DeleteValue(AppPreferences.AppName, false);
            }
        }
    }
}
