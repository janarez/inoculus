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

        public CountDownCircle(double totalMiliseconds)
        {
            // Start at the top, then go clockwise every `seconds_per_step`.
            angle_step = 2 * Math.PI / (totalMiliseconds / AppPreferences.CountDownCircleSpeed);

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
            endPoint.X = AppPreferences.CircleRadius + 0.001;  // Add little offset to force paint of full circle.
            endPoint.Y = AppPreferences.CircleHalfThickness;
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(EndPoint)));
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void Dispose()
        {
            timer.Dispose();
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
            double x = Math.Abs(Math.Sin(current_angle)) * (AppPreferences.CircleRadius - AppPreferences.CircleHalfThickness);
            x = isInRightHalf? AppPreferences.CircleRadius + x : AppPreferences.CircleRadius - x;
            endPoint.X = x;
        }
        private void UpdateY()
        {
            double y = Math.Abs(Math.Cos(current_angle)) * (AppPreferences.CircleRadius - AppPreferences.CircleHalfThickness);
            y = isInBottomHalf ? y = AppPreferences.CircleRadius + y : AppPreferences.CircleRadius - y;
            endPoint.Y = y;
        }
    }
}
