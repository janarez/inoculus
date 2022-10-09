using System;
using System.Windows;

namespace InOculus.Utilities
{
    class UserPreferences
    {
        public static DisplayedTimeSpan FocusInterval = new DisplayedTimeSpan(minutes: 0, seconds: 10);  //new TimeSpan(hours: 0, minutes: Properties.Settings.Default.FocusInterval, seconds: 0);
        public static TimeSpan BreakInterval = new TimeSpan(hours: 0, minutes: 0, seconds: 3);  //new TimeSpan(hours: 0, minutes: 0, seconds:  Properties.Settings.Default.BreakInterval);
    }

    class AppPreferences
    {
        public static double CountDownCircleSpeed = 100;
        public static double IntervalTimerSpeed = 1000;

        public static double WindowWidth = 200;
        public static double WindowHeight = 240;

        public static double CircleDiameter = 150;
        public static double CircleThickness = 10;

        // Helper variables for XAML binding.
        public static double CircleRadius = CircleDiameter / 2;
        public static double CircleHalfThickness = CircleThickness / 2;
        public static Thickness BorderThickness = new Thickness(CircleThickness);
        public static Thickness NegBorderThickness = new Thickness(-CircleThickness);
        public static Point CircleStartPoint = new Point(CircleRadius, CircleHalfThickness);
        public static Size CircleSize = new Size(CircleRadius - CircleHalfThickness, CircleRadius - CircleHalfThickness);
    }
}
