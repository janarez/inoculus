using System;
using System.ComponentModel;
using System.Timers;

namespace InOculus.Utilities
{
    class IntervalTimer : INotifyPropertyChanged
    {
        private static readonly TimeSpan interval = new TimeSpan(hours: 0, minutes: 0, seconds: 10);  //new TimeSpan(hours: 0, minutes: Properties.Settings.Default.Interval, seconds: 0);
        private readonly Timer timer = new Timer(1000);

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

        public IntervalTimer()
        {
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CountDown += TimeSpan.FromSeconds(-1);

            if (CountDown.TotalSeconds == 0)
            {
                Reset();
                Start();
            }
        }

        public void Start()
        {
            timer.Start();
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
