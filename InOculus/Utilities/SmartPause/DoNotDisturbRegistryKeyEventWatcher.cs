using System.Management;

namespace InOculus.Utilities.SmartPause
{
    internal class DoNotDisturbRegistryKeyEventWatcher : BaseRegistryKeyEventWatcher
    {
        private const string doNotDisturbRegistryValueName = "LastUsedTimeStop";


        public DoNotDisturbRegistryKeyEventWatcher(string registryKeyPath) : base(registryKeyPath, doNotDisturbRegistryValueName)
        {
        }

        protected override void registryKeyChangedEventHandler(object sender, EventArrivedEventArgs e)
        {
            // Implement custom logic here
        }
    }
}
