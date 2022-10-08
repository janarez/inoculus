using System;

namespace InOculus.Utilities
{
    class UserPreferences
    {
        public static TimeSpan FocusInterval = new TimeSpan(hours: 0, minutes: 0, seconds: 10);  //new TimeSpan(hours: 0, minutes: Properties.Settings.Default.FocusInterval, seconds: 0);
        public static TimeSpan BreakInterval = new TimeSpan(hours: 0, minutes: 0, seconds: 3);  //new TimeSpan(hours: 0, minutes: 0, seconds:  Properties.Settings.Default.BreakInterval);
    }

    class AppPreferences
    {
        public static double CountDownCircleSpeed = 100;
        public static double IntervalTimerSpeed = 1000;

    }
}
