using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;

namespace InOculus.Utilities
{
    class CountDownCircle : INotifyPropertyChanged
    {
        private readonly Timer timer = new Timer(AppPreferences.CountDownCircleSpeed);

        private readonly double circle_radius;    // Radius from outer edge to center.
        private readonly double circle_thickness; // Width of circle line.
        private readonly double angle_step;

        private double current_angle;
        private bool isInRightHalf;
        private bool isInBottomHalf;
        
        private Point endPoint;
        public Point EndPoint { get => endPoint; set => endPoint = value; }
        private bool isLargeArc;
        public bool IsLargeArc
        {
            get => isLargeArc;
            private set
            {
                isLargeArc = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsLargeArc)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CountDownCircle(double circle_diameter, double circle_thickness)
        {
            circle_radius = circle_diameter / 2;
            this.circle_thickness = circle_thickness;

            // Start at the top, then go clockwise every `seconds_per_step`.
            angle_step = 2 * Math.PI / (UserPreferences.FocusInterval.TotalMilliseconds / AppPreferences.CountDownCircleSpeed);

            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SetPosition();
            UpdateX();
            UpdateY();
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(EndPoint)));
        }

        public void Start()
        {
            current_angle = 0;
            IsLargeArc = true;
            endPoint.X = circle_radius + 0.001;  // Add little offset to force paint of full circle.
            endPoint.Y = circle_thickness / 2;
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(EndPoint)));
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void SetPosition()
        {
            current_angle += angle_step;
            if (IsLargeArc && current_angle > Math.PI)
            {
                IsLargeArc = false;
            }

            isInRightHalf = current_angle < Math.PI;
            isInBottomHalf = (current_angle > Math.PI / 2) && (current_angle < (Math.PI + Math.PI / 2));
        }

        private void UpdateX()
        {
            double x = Math.Abs(Math.Sin(current_angle)) * (circle_radius - circle_thickness / 2);
            x = isInRightHalf? circle_radius + x : circle_radius - x;
            Debug.WriteLine($"x: {x}");
            endPoint.X = x;
        }
        private void UpdateY()
        {
            double y = Math.Abs(Math.Cos(current_angle)) * (circle_radius - circle_thickness / 2);
            Debug.WriteLine($"y: {y}");
            y = isInBottomHalf ? y = circle_radius + y : circle_radius - y;
            Debug.WriteLine($"y: {y}");
            endPoint.Y = y;
        }
    }
}
