﻿using System;
using System.ComponentModel;
using System.Timers;

namespace InOculus.Utilities
{
    class IntervalTimer : INotifyPropertyChanged
    {
        private readonly Timer timer = new Timer(AppPreferences.IntervalTimerSpeed);

        private readonly DisplayedTimeSpan countDownInterval;
        private DisplayedTimeSpan countDown;
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


        public IntervalTimer(DisplayedTimeSpan countDownInterval)
        {
            this.countDownInterval = countDownInterval;
            countDown = countDownInterval;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CountDown += TimeSpan.FromSeconds(-AppPreferences.IntervalTimerSpeed / 1000);
        }

        public void Start()
        {
            CountDown = countDownInterval;
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            CountDown = countDownInterval;
        }

        public void Close()
        {
            timer.Close();
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }

    public class DisplayedTimeSpan
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
            return $"{(int)TimeSpan.TotalMinutes:D2}:{TimeSpan.Seconds:D2}";
        }

        public static DisplayedTimeSpan operator +(DisplayedTimeSpan t1, TimeSpan t2)
        {
            return new DisplayedTimeSpan(t1.TimeSpan + t2);
        }

    }

}
