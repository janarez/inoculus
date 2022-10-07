using System;
using System.ComponentModel;
using System.Diagnostics;
using static System.Threading.Thread;
using System.Timers;

namespace InOculus.Utilities
{
    class IntervalTimer : INotifyPropertyChanged
    {
        private static readonly TimeSpan interval = new TimeSpan(hours: 0, minutes: 0, seconds: 60);  //new TimeSpan(hours: 0, minutes: Properties.Settings.Default.Interval, seconds: 0);
        private static readonly double seconds_per_step = 0.1;

        private readonly Timer timer = new Timer(seconds_per_step * 1000);

        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsOn = false;

        private TimeSpan countDown = interval;
        public TimeSpan CountDown
        {
            get => countDown;
            private set
            {
                countDown = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CountDown)));
            }
        }
        public CountDownCircle CountDownCircle = new CountDownCircle(total_seconds: interval.TotalSeconds, seconds_per_step: seconds_per_step, circle_diameter: 150, circle_thickness: 10);

        public IntervalTimer()
        {
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CountDown += TimeSpan.FromSeconds(-seconds_per_step);
            CountDownCircle.Tick();
            
            Debug.WriteLine($"{CountDown.TotalSeconds}: ({ (int)CountDownCircle.EndPoint.X}, {(int)CountDownCircle.EndPoint.Y})");

            if (CountDown.TotalSeconds == 0)
            {
                Reset();
                Sleep(2000);
                Start();
            }
        }

        public void Start()
        {
            timer.Start();
            CountDownCircle.Start();
            IsOn = true;
        }

        public void Reset()
        {
            timer.Stop();
            CountDown = interval;
            IsOn = false;
        }
    }
}
