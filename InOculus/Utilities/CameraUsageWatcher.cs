using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;

namespace InOculus.Utilities
{
    internal class CameraUsageWatcher
    {
        private readonly List<CameraAppManagementEventWatcher> cameraEventWatchers = new();

        public CameraUsageWatcher()
        {
            // Cannot use HKEY_CURRENT_USER directly as it's not tracked by `RegistryEvent`, so we need to look through HKEY_USERS.
            var currentUserKey = WindowsIdentity.GetCurrent().User?.Value;

            if (currentUserKey != null)
            {
                // All apps with camera access should be here except for system apps that are in one level up (i.e. not in `NonPackaged`).
                var cameraAccessKeys = $"{currentUserKey}\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\webcam\\NonPackaged";
                var parentRegistryKey = Registry.Users.OpenSubKey(cameraAccessKeys);

                if (parentRegistryKey != null)
                {
                    foreach (var appKey in parentRegistryKey.GetSubKeyNames())
                    {
                        var registryKeyPath = $"{cameraAccessKeys}\\{appKey}";
                        var cameraWatcher = new CameraAppManagementEventWatcher(registryKeyPath);
                        cameraEventWatchers.Add(cameraWatcher);
                        cameraWatcher.CameraStateChanged += CameraWatcher_CameraStateChanged;
                    }

                    parentRegistryKey.Close();
                }
            }
        }
        private void CameraWatcher_CameraStateChanged(object sender, CameraAppManagementEventWatcher.CameraStateChangedEventArgs e)
        {
            Debug.WriteLine(e.AppStateToTrigger);
        }

        public void Dispose()
        {
            foreach (var cameraEventWatcher in cameraEventWatchers)
            {
                cameraEventWatcher.Dispose();
            }
        }
    }
}
