using System;

namespace InOculus.Utilities.SmartPause
{
    internal class SmartPauseEventArgs : EventArgs
    {
        public SmartPauseState SmartPauseState { get; }

        public SmartPauseEventArgs(SmartPauseState smartPauseState)
        {
            SmartPauseState = smartPauseState;
        }
    }
}
