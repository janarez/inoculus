using System;

namespace InOculus.Utilities.SmartPause
{
    internal class SmartPauseEventArgs : EventArgs
    {
        public AppState AppStateToTrigger { get; }

        public SmartPauseEventArgs(AppState appStateToTrigger)
        {
            AppStateToTrigger = appStateToTrigger;
        }
    }
}
