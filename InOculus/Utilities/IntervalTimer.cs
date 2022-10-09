using System;
using System.ComponentModel;
using System.Timers;

namespace InOculus.Utilities
{
    class IntervalTimer : INotifyPropertyChanged
    {
        private readonly Timer timer = new Timer(AppPreferences.IntervalTimerSpeed);

        private DisplayedTimeSpan countDown = UserPreferences.FocusInterval;
        public DisplayedTimeSpan CountDown
        {
            get => countDown;
            set
            {
                countDown = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CountDown)));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;


        public IntervalTimer()
        {
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CountDown += TimeSpan.FromSeconds(-AppPreferences.IntervalTimerSpeed / 1000);
        }

        public void Start()
        {
            CountDown = UserPreferences.FocusInterval;
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }

    class DisplayedTimeSpan
    {
        public DisplayedTimeSpan(int minutes, int seconds)
        {
            TimeSpan = new TimeSpan(hours: 0, minutes: minutes, seconds: seconds);
        }
        public DisplayedTimeSpan(TimeSpan t)
        {
            TimeSpan = t;
        }

        public TimeSpan TimeSpan;

        public override string ToString()
        {
            return $"{(int)TimeSpan.TotalMinutes} m {TimeSpan.Seconds} s";
        }

        public static DisplayedTimeSpan operator +(DisplayedTimeSpan t1, TimeSpan t2)
        {
            return new DisplayedTimeSpan(t1.TimeSpan + t2);
        }

    }

}
