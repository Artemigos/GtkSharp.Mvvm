using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace GtkSharp.Mvvm.Bindings
{
    public class ErrorsBindingTarget : IBindingTarget
    {
        private readonly INotifyDataErrorInfo target;
        private readonly string propertyName;
        private readonly List<Action<object>> subscriptions;
        private readonly PropertyInfo property;
        private bool connected = false;

        public ErrorsBindingTarget(INotifyDataErrorInfo target, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or empty.", nameof(propertyName));
            }

            this.target = target ?? throw new ArgumentNullException(nameof(target));
            this.propertyName = propertyName;
            this.subscriptions = new List<Action<object>>();

            this.property = target.GetType().GetProperty(propertyName);
            if (this.property is null)
            {
                throw new ArgumentException("Property does not exist on target.", nameof(propertyName));
            }
        }

        public bool CanTrack => true;

        public void Connect()
        {
            if (this.connected)
            {
                throw new InvalidOperationException("Cannot connect twice.");
            }

            this.connected = true;
            this.target.ErrorsChanged += this.HandleErrorsChanged;
        }

        public void Disconnect()
        {
            if (!this.connected)
            {
                throw new InvalidOperationException("Cannot connect when disconnected.");
            }

            this.connected = false;
            this.target.ErrorsChanged -= this.HandleErrorsChanged;
        }

        public object GetValue()
        {
            return this.target.GetErrors(this.propertyName);
        }

        public bool IsDependentOn(object dependency)
        {
            if (dependency is null)
            {
                throw new ArgumentNullException(nameof(dependency));
            }

            return ReferenceEquals(dependency, this.target);
        }

        public void SetValue(object value)
        {
            throw new NotSupportedException();
        }

        public void Subscribe(Action<object> handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            this.subscriptions.Add(handler);
        }

        public void Unsubscribe(Action<object> handler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            this.subscriptions.Remove(handler);
        }

        private void HandleErrorsChanged(object sender, DataErrorsChangedEventArgs args)
        {
            if (args.PropertyName == this.propertyName)
            {
                foreach (var subscription in this.subscriptions)
                {
                    subscription(this.GetValue());
                }
            }
        }
    }
}

