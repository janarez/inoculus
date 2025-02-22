using System;

namespace InOculus.Utilities.SmartPause
{
    internal interface ISmartPauseWatcher : IDisposable
    {
        event EventHandler SmartPauseStateChanged;
        SmartPauseState GetSmartPauseState();
    }


    internal enum SmartPauseState
    {
        On,
        Off
    }
}
