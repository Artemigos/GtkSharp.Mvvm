using System;

namespace GtkSharp.Mvvm.Bindings
{
    public interface IBindingTarget
    {
        void Connect();
        void Disconnect();
        object GetValue();
        void SetValue(object value);
        void Subscribe(Action<object> handler);
        void Unsubscribe(Action<object> handler);
        bool IsDependentOn(object dependency);
        bool CanTrack { get; }
    }
}
