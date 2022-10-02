using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Timers;

namespace InOculus.Utilities
{
    class IntervalTimer: INotifyPropertyChanged
    {
        private Timer timer = new Timer(1000);
        public event PropertyChangedEventHandler PropertyChanged;
        public TimeSpan CountDown { get; private set; } = new TimeSpan(hours: 0, minutes: Properties.Settings.Default.Interval, seconds: 0);

        public IntervalTimer()
        {
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CountDown += TimeSpan.FromSeconds(-1);
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(CountDown)));
        }

        public void Start()
        {
            timer.Start();
        }
    }
}
