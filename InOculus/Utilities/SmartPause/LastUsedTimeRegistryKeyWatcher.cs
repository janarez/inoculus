using System;
using System.Management;

namespace InOculus.Utilities.SmartPause
{
    internal class LastUsedTimeRegistryKeyWatcher : BaseRegistryKeyEventWatcher, ISmartPauseWatcher
    {
        public event EventHandler SmartPauseStateChanged;

        private const string appLastStartRegistryValueName = "LastUsedTimeStart";
        private const string appLastStopRegistryValueName = "LastUsedTimeStop";


        public LastUsedTimeRegistryKeyWatcher(string registryKeyPath) : base(registryKeyPath, appLastStopRegistryValueName)
        {

        }


        protected override void registryKeyChangedEventHandler(object sender, EventArrivedEventArgs e)
        {
            SmartPauseStateChanged?.Invoke(this, new EventArgs());
        }

        public SmartPauseState GetSmartPauseState()
        {
            var startTimestampObject = registryKey.GetValue(appLastStartRegistryValueName);
            var startTimestamp = convertActiveDirectoryTimeToDateTime(startTimestampObject);

            var stopTimestampObject = registryKey.GetValue(appLastStopRegistryValueName);
            var stopTimestamp = convertActiveDirectoryTimeToDateTime(stopTimestampObject);

            if (DateTime.Now > startTimestamp && stopTimestamp == null)
            {
                return SmartPauseState.On;
            }

            return SmartPauseState.Off;
        }

        private static DateTime? convertActiveDirectoryTimeToDateTime(object activeDirectoryTimeObject)
        {
            var activeDirectoryTime = Convert.ToInt64(activeDirectoryTimeObject);

            if (activeDirectoryTime == 0)
            {
                return null;
            }
            return DateTime.FromFileTimeUtc((long)activeDirectoryTimeObject);
        }
    }
}
