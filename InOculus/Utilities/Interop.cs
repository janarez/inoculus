using System;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace InOculus.Utilities
{
    static class Interop
    {
        private const uint WS_MINIMIZE = 0x20000000;

        /// <summary>
        /// Hacky way to force focus the specified window.
        /// </summary>
        /// <remarks>
        /// From <see href="https://stackoverflow.com/a/43119811"/>.
        /// </remarks>
        public static unsafe void FocusWindow(IntPtr focusOnWindowHandle)
        {
            var handle = new HWND(focusOnWindowHandle);
            int style = GetWindowLong(handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

            // Restore to be able to make it active.
            if ((style & WS_MINIMIZE) == WS_MINIMIZE)
            {
                ShowWindow(handle, SHOW_WINDOW_CMD.SW_RESTORE);
            }

            uint currentlyFocusedWindowProcessId = GetWindowThreadProcessId(GetForegroundWindow());
            uint appThread = GetCurrentThreadId();

            if (currentlyFocusedWindowProcessId != appThread)
            {
                AttachThreadInput(currentlyFocusedWindowProcessId, appThread, true);
                BringWindowToTop(handle);
                ShowWindow(handle, SHOW_WINDOW_CMD.SW_SHOW);
                AttachThreadInput(currentlyFocusedWindowProcessId, appThread, false);
            }
            else
            {
                BringWindowToTop(handle);
                ShowWindow(handle, SHOW_WINDOW_CMD.SW_SHOW);
            }
        }
    }
}
