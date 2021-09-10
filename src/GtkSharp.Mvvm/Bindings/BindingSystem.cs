using System.Collections.Generic;

namespace GtkSharp.Mvvm.Bindings
{
    public static class BindingSystem
    {
        static BindingSystem()
        {
            TrackingMethods.Add(new PropertyChangedTrackingMethod());
            TrackingMethods.Add(new ErrorsChangedTrackingMethod());
            TrackingMethods.Add(new CanExecuteChangedTrackingMethod());
            TrackingMethods.Add(new GlibNotificationTrackingMethod());
        }

        public static IList<ITrackingMethod> TrackingMethods { get; } = new List<ITrackingMethod>();
    }
}
