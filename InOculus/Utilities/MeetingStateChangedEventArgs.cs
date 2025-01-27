using System;

namespace InOculus.Utilities
{
    internal class MeetingStateChangedEventArgs : EventArgs
    {
        public AppState AppStateToTrigger { get; }

        public MeetingStateChangedEventArgs(AppState appStateToTrigger)
        {
            AppStateToTrigger = appStateToTrigger;
        }
    }
}
