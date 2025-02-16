using System;
using System.Management;

namespace InOculus.Utilities
{
    internal class PeripheralUseRegistryKeyEventWatcher : BaseRegistryKeyEventWatcher
    {
        private const string appLastStartRegistryValueName = "LastUsedTimeStart";
        private const string appLastStopRegistryValueName = "LastUsedTimeStop";


        public PeripheralUseRegistryKeyEventWatcher(string registryKeyPath) : base(registryKeyPath, appLastStopRegistryValueName)
        {

        }

        protected override void registryKeyChangedEventHandler(object sender, EventArrivedEventArgs e)
        {
            var startTimestampObject = registryKey.GetValue(appLastStartRegistryValueName);
            var startTimestamp = convertActiveDirectoryTimeToDateTime(startTimestampObject);

            var stopTimestampObject = registryKey.GetValue(appLastStopRegistryValueName);
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
    }
}
