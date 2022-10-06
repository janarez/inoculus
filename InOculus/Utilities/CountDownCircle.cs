using System;
using System.ComponentModel;
using System.Windows;

namespace InOculus.Utilities
{
    class CountDownCircle : INotifyPropertyChanged
    {
        private readonly int circle_diameter;  // Diameter between outer edges.
        private readonly int circle_thickness; // Width of circle line.

        private double current_angle;
        private readonly double angle_step;
        private bool isInTopHalf = true;
        private bool isInRightHalf = true;

        public Point EndPoint;

        public event PropertyChangedEventHandler PropertyChanged;

        public CountDownCircle(double total_seconds, int seconds_per_step, int circle_diameter, int circle_thickness)
        {
            this.circle_diameter = circle_diameter;
            this.circle_thickness = circle_thickness;

            // Start at the top, then go clockwise every `seconds_per_step`.
            current_angle = 0;
            angle_step = 2 * Math.PI / (total_seconds / seconds_per_step);
        }

        public void Tick()
        {
            current_angle += angle_step;
            UpdateX();
            UpdateY();
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(EndPoint)));
        }

        private void UpdateX()
        {
            double x = Math.Sin(current_angle) * (circle_diameter - circle_thickness);

            if (isInRightHalf)
            {
                x = circle_diameter + x;
                if(current_angle + angle_step > Math.PI) {
                    isInRightHalf = false;
                }
            }
            else
            {
                x = circle_diameter - x;
            }

            EndPoint.X = x;
        }
        private void UpdateY()
        {
            double y = Math.Cos(current_angle) * (circle_diameter - circle_thickness);

            if (isInTopHalf)
            {
                y = circle_diameter - y;
                if (current_angle + angle_step > Math.PI / 2)
                {
                    isInTopHalf = false;
                }
            }
            else
            {
                y = circle_diameter + y;
                if (current_angle + angle_step > (Math.PI + Math.PI / 2))
                {
                    isInTopHalf = true;
                }
            }

            EndPoint.Y = y;
        }
    }
}
