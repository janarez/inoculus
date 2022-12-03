using System;
using System.Windows;

namespace InOculus.Utilities
{
    class UserPreferences
    {
#if DEBUG
        public static DisplayedTimeSpan FocusInterval = new DisplayedTimeSpan(minutes: 0, seconds: 5);  
        public static DisplayedTimeSpan BreakInterval = new DisplayedTimeSpan(minutes: 0, seconds: 10);
#else
        public static DisplayedTimeSpan FocusInterval = new DisplayedTimeSpan(minutes: Properties.Settings.Default.FocusInterval, seconds: 0);
        public static DisplayedTimeSpan BreakInterval = new DisplayedTimeSpan(minutes: 0, seconds:  Properties.Settings.Default.BreakInterval);
#endif
    }

    class AppPreferences
    {
        public const string AppName = "InOculus";
        public const double CountDownCircleSpeed = 100;
        public const double IntervalTimerSpeed = 1000;

        public const double WindowWidth = 200;
        public const double WindowHeight = 240;
               
        public const double CircleDiameter = 150;
        public const double CircleThickness = 10;

        // Helper variables for XAML binding.
        public const double CircleRadius = CircleDiameter / 2;
        public const double CircleHalfThickness = CircleThickness / 2;
        public const double CountDownDiameter = CircleDiameter + 2;
        public static Thickness BorderThickness = new Thickness(CircleThickness);
        public static Thickness NegBorderThickness = new Thickness(-CircleThickness);
        public static Point CircleStartPoint = new Point(CircleRadius, CircleHalfThickness);
        public static Size CircleSize = new Size(CircleRadius - CircleHalfThickness, CircleRadius - CircleHalfThickness);
    }
}
