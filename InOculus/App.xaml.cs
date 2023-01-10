using InOculus.Utilities;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace InOculus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public AppState State;

        private Mutex mutex;
        private EventWaitHandle eventWaitHandle;

# if DEBUG
        private const string mutexName = $"{AppPreferences.AppName}_mutex_debug";
        private const string handleName = $"{AppPreferences.AppName}_handle_debug";
#else
        private const string mutexName = $"{AppPreferences.AppName}_mutex";
        private const string handleName = $"{AppPreferences.AppName}_handle";
#endif

        private void AppOnStartup(object sender, StartupEventArgs e)
        {
            mutex = new Mutex(true, mutexName, out bool createdNew);
            eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, handleName);

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

        public void SetState(AppState state)
        {
            string icon = state switch
            {
                AppState.Stop => "InOculus-stop.ico",
                AppState.Pause => "InOculus-stop.ico",
                _ => "InOculus.ico"
            };
            State = state;
            Uri iconUri = new Uri($"pack://application:,,,/{icon}", UriKind.Absolute);
            MainWindow.Icon = BitmapFrame.Create(iconUri);
        }
    }

    public enum AppState
    {
        Focus,
        Break,
        Pause,
        Stop,
    }

}
