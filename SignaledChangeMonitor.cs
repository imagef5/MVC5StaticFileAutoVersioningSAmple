using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Caching;

namespace AutoVersioningSample
{
    public class SignaledChangeEventArgs : EventArgs
    {
        public string Name { get; private set; }
        public SignaledChangeEventArgs(string name = null)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Cache change monitor that allows an app to fire a change notification
    /// to all associated cache items.
    /// </summary>
    public class SignaledChangeMonitor : ChangeMonitor
    {
        // Shared across all SignaledChangeMonitors in the AppDomain
        private static event EventHandler<SignaledChangeEventArgs> Signaled;

        private string _name;
        private string _uniqueId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

        public override string UniqueId
        {
            get { return _uniqueId; }
        }

        public SignaledChangeMonitor(string name = null)
        {
            _name = name;
            // Register instance with the shared event
            Signaled += OnSignalRaised;
            base.InitializationComplete();
        }

        public static void Signal(string name = null)
        {
            if (Signaled != null)
            {
                // Raise shared event to notify all subscribers
                Signaled(null, new SignaledChangeEventArgs(name));
            }
        }

        protected override void Dispose(bool disposing)
        {
            Signaled -= OnSignalRaised;
        }

        private void OnSignalRaised(object sender, SignaledChangeEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Name) || string.Compare(e.Name, _name, true) == 0)
            {
                Debug.WriteLine(
                    _uniqueId + " notifying cache of change.", "SignaledChangeMonitor");
                // Cache objects are obligated to remove entry upon change notification.
                base.OnChanged(null);
            }
        }
    }
}
