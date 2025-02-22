using System;
using System.Timers;

namespace InOculus.Utilities.SmartPause
{
    internal class DoNotDisturbModePoller : ISmartPauseWatcher
    {
        public event EventHandler SmartPauseStateChanged;
        private Timer pollerTimer;
        private SmartPauseState currentState = SmartPauseState.Off;

        public DoNotDisturbModePoller()
        {
            // Poll once a minute.
            pollerTimer = new Timer(1_000);
            pollerTimer.Elapsed += PollerTimer_Elapsed;
            pollerTimer.Start();
        }

        private SmartPauseState pollSmartPauseState()
        {
            DoNotDisturbInterop.GetDoNotDisturbMode(out var focusAssistMode);
            if (focusAssistMode == DoNotDisturbMode.PriorityOnly || focusAssistMode == DoNotDisturbMode.AlarmOnly)
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
            return pollSmartPauseState();
        }

        public void Dispose()
        {
            pollerTimer.Stop();
            pollerTimer.Dispose();
        }
    }
}
