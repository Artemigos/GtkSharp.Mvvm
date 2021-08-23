using System;
using System.ComponentModel;

namespace GtkSharp.Mvvm.Bindings
{
    public class NotifyBindingTarget : IBindingTarget
    {
        public NotifyBindingTarget(INotifyPropertyChanged target, string propertyName)
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public object GetValue()
        {
            throw new NotImplementedException();
        }

        public bool IsDependentOn(object dependency)
        {
            throw new NotImplementedException();
        }

        public void SetValue(object value)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Action<object> handler)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(Action<object> handler)
        {
            throw new NotImplementedException();
        }
    }
}
