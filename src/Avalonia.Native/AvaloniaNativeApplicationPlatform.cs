using System;
using System.ComponentModel;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Native.Interop;
using Avalonia.Platform;

namespace Avalonia.Native
{
    internal class AvaloniaNativeApplicationPlatform : NativeCallbackBase, IAvnApplicationEvents, IPlatformLifetimeEventsImpl
    {
        public event EventHandler<ShutdownRequestedEventArgs> ShutdownRequested;
        
        void IAvnApplicationEvents.FilesOpened(IAvnStringArray urls)
        {
            ((IApplicationPlatformEvents)Application.Current).RaiseUrlsOpened(urls.ToStringArray());

            if (Application.Current?.ApplicationLifetime is MacOSClassicDesktopStyleApplicationLifetime lifetime)
            {
                foreach (var url in urls.ToStringArray())
                {
                    lifetime.RaiseUrl(new Uri(url));
                }
            }
        }

        void IAvnApplicationEvents.OnReopen()
        {
            if (Application.Current?.ApplicationLifetime is MacOSClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.RaiseActivated(ActivationKind.Reopen);    
            }
        }

        void IAvnApplicationEvents.OnHide()
        {
            if (Application.Current?.ApplicationLifetime is MacOSClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.RaiseDeactivated(ActivationKind.Background);    
            }
        }

        void IAvnApplicationEvents.OnUnhide()
        {
            if (Application.Current?.ApplicationLifetime is MacOSClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.RaiseActivated(ActivationKind.Background);    
            }
        }

        public int TryShutdown()
        {
            if (ShutdownRequested is null) return 1;
            var e = new ShutdownRequestedEventArgs();
            ShutdownRequested(this, e);
            return (!e.Cancel).AsComBool();
        }
    }
}
