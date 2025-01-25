using System;

namespace InOculus.Utilities
{
    internal class CameraStateChangedEventArgs : EventArgs
    {
        public AppState AppStateToTrigger { get; }

        public CameraStateChangedEventArgs(AppState appStateToTrigger)
        {
            AppStateToTrigger = appStateToTrigger;
        }
    }
}
