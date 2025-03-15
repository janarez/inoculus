using System;
using System.Management;

namespace InOculus.Utilities.SmartPause
{
    internal class ProcessWatcher : ISmartPauseWatcher
    {
        private readonly ManagementEventWatcher _startWatcher, _stopWatcher;
        private bool _processRunning;

        public event EventHandler SmartPauseStateChanged;

        public ProcessWatcher(string processName)
        {
            _startWatcher = WatchForProcessStart(processName);
            _stopWatcher = WatchForProcessStop(processName);
        }

        public void Dispose()
        {
            _startWatcher.Dispose();
            _stopWatcher.Dispose();
        }

        public SmartPauseState GetSmartPauseState()
        {
            return _processRunning ? SmartPauseState.On : SmartPauseState.Off;
        }

        private ManagementEventWatcher WatchForProcessStart(string processName)
        {
            WqlEventQuery query = new WqlEventQuery(
                $"SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process' AND TargetInstance.Name = '{processName}'");

            ManagementEventWatcher watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += (sender, e) =>
            {
                _processRunning = true;
                SmartPauseStateChanged?.Invoke(this, e);
            };
            watcher.Start();
            return watcher;
        }

        private ManagementEventWatcher WatchForProcessStop(string processName)
        {
            WqlEventQuery query = new WqlEventQuery(
                $"SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process' AND TargetInstance.Name = '{processName}'");

            ManagementEventWatcher watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += (sender, e) =>
            {
                _processRunning = false;
                SmartPauseStateChanged?.Invoke(this, e);
            };
            watcher.Start();
            return watcher;
        }
    }
}
