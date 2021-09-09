using System.Collections.Generic;

namespace GtkSharp.Mvvm.Bindings
{
    public static class BindingSystem
    {
        static BindingSystem()
        {
            TrackingMethods.Add(new PropertyChangedTrackingMethod());
        }

        public static IList<ITrackingMethod> TrackingMethods { get; } = new List<ITrackingMethod>();
    }
}
