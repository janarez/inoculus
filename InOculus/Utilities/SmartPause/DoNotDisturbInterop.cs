using System;
using System.Runtime.InteropServices;

namespace InOculus.Utilities.SmartPause
{
    // https://stackoverflow.com/a/53411482
    // https://stackoverflow.com/a/77767799
    // Do not disturb (also quiet hours, focus mode).
    internal static class DoNotDisturbInterop
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int ZwQueryWnfStateData(
            ref ulong StateName,
            Guid TypeId,
            IntPtr ExplicitScope,
            out uint ChangeStamp,
            IntPtr Buffer,
            ref int BufferSize
        );

        const ulong WNF_SHEL_QUIETHOURS_ACTIVE_PROFILE_CHANGED = 0xd83063ea3bf1C75UL;

        public static int GetDoNotDisturbMode(out DoNotDisturbMode mode)
        {
            var stateName = WNF_SHEL_QUIETHOURS_ACTIVE_PROFILE_CHANGED;

            int bufferSize = 4;
            var bufferPtr = Marshal.AllocHGlobal(bufferSize);
            int result;

            try
            {
                result = ZwQueryWnfStateData(ref stateName, Guid.Empty, IntPtr.Zero, out _, bufferPtr, ref bufferSize);

                if (result == 0)
                {
                    var modeAsByteArray = new byte[bufferSize];

                    Marshal.Copy(bufferPtr, modeAsByteArray, 0, modeAsByteArray.Length);

                    mode = (DoNotDisturbMode)BitConverter.ToUInt32(modeAsByteArray);
                }
                else
                {
                    mode = DoNotDisturbMode.Off;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(bufferPtr);
            }

            return result;
        }
    }

    internal enum DoNotDisturbMode : int
    {
        NotSupported = -2,
        Failed = -1,
        Off = 0,
        PriorityOnly = 1,
        AlarmOnly = 2,
    }
}
