﻿using InOculus.Utilities;
using System;
using System.Threading;
using System.Windows;

namespace InOculus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex mutex;
        private EventWaitHandle eventWaitHandle;


        private void AppOnStartup(object sender, StartupEventArgs e)
        {
            mutex = new Mutex(true, $"{AppPreferences.AppName}_mutex", out bool createdNew);
            eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, $"{AppPreferences.AppName}_handle");

            // First instance.
            if (createdNew)
            {   
                // Spawn thread that will wait for launch of another app instance 
                // and bring main window of existing one to foreground.
                var thread = new Thread(
                    () =>
                    {
                        while (eventWaitHandle.WaitOne())
                        {
                            Current.Dispatcher.BeginInvoke(() => ((MainWindow)Current.MainWindow).ToForeground());
                        }
                    });

                thread.IsBackground = true;
                thread.Start();
                return;
            }
            // Do not open second instance, rather activate first.
            eventWaitHandle.Set();
            Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            mutex.Dispose();
            eventWaitHandle.Dispose();
            base.OnExit(e);
        }
    }
}
