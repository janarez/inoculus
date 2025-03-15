using System;
using System.Timers;

namespace InOculus.Utilities.SmartPause
{
    internal class DoNotDisturbModePoller : ISmartPauseWatcher
    {
        public event EventHandler SmartPauseStateChanged;
        private Timer pollerTimer;
        private SmartPauseState currentState = SmartPauseState.Unknown;

        public DoNotDisturbModePoller()
        {
            pollerTimer = new Timer(60_000);
            pollerTimer.Elapsed += PollerTimer_Elapsed;
            pollerTimer.Start();
        }

        private SmartPauseState pollSmartPauseState()
        {
            DoNotDisturbInterop.GetDoNotDisturbMode(out var focusAssistMode);
            if (focusAssistMode is DoNotDisturbMode.PriorityOnly or DoNotDisturbMode.AlarmOnly)
            {
                return SmartPauseState.On;
            }
            return SmartPauseState.Off;
        }

        private void PollerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var newState = pollSmartPauseState();

            if (newState != currentState)
            {
                currentState = newState;
                SmartPauseStateChanged?.Invoke(this, new EventArgs());
            }
        }

        public SmartPauseState GetSmartPauseState()
        {
            return currentState;
        }

        public void Dispose()
        {
            pollerTimer.Stop();
            pollerTimer.Dispose();
        }
    }
}
