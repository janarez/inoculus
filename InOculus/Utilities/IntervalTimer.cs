using System;
using System.ComponentModel;
using System.Diagnostics;
using static System.Threading.Thread;
using System.Timers;

namespace InOculus.Utilities
{
    class IntervalTimer : INotifyPropertyChanged
    {
        private readonly Timer timer = new Timer(AppPreferences.IntervalTimerSpeed);

        public event PropertyChangedEventHandler PropertyChanged;

        private TimeSpan _countDown = UserPreferences.FocusInterval;
        private TimeSpan countDown
        {
            get => _countDown;
            set
            {
                _countDown = value;
                if (_countDown.Milliseconds == 0)
                {
                    CountDownString = $"{(int)_countDown.TotalMinutes} m {_countDown.Seconds} s";
                }
            }
        }
        private string countDownString;
        public string CountDownString
        {
            get => countDownString;
            private set
            {
                countDownString = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CountDownString)));
            }
        }

        public IntervalTimer()
        {
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            countDown += TimeSpan.FromSeconds(-AppPreferences.IntervalTimerSpeed / 1000);
        }

        public void Start()
        {
            countDown = UserPreferences.FocusInterval;
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }
}
