using System;
using System.Collections.Generic;
using System.Reflection;

namespace GtkSharp.Mvvm.Bindings
{
    public class GObjectBindingTarget : IBindingTarget
    {
        private readonly GLib.Object target;
        private readonly string propertyName;
        private readonly List<Action<object>> subscriptions;
        private readonly PropertyInfo property;
        private readonly GLib.PropertyAttribute attribute;
        private bool connected;

        public GObjectBindingTarget(GLib.Object target, string propertyName)
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

            this.attribute = this.property.GetCustomAttribute<GLib.PropertyAttribute>();
        }

        public bool CanTrack => this.attribute is not null;

        public void Connect()
        {
            if (this.connected)
            {
                throw new InvalidOperationException("Cannot connect twice.");
            }

            if (!this.CanTrack)
            {
                throw new InvalidOperationException("Cannot connect to an untrackable property.");
            }

            this.connected = true;
            this.target.AddNotification(this.attribute.Name, this.HandleNotification);
        }

        public void Disconnect()
        {
            if (!this.connected)
            {
                throw new InvalidOperationException("Cannot connect when disconnected.");
            }

            this.connected = false;
            this.target.RemoveNotification(this.attribute.Name, this.HandleNotification);
        }

        public object GetValue()
        {
            return this.property.GetValue(this.target);
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
            this.property.SetValue(this.target, value);
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

        private void HandleNotification(object o, GLib.NotifyArgs args)
        {
            foreach (var subscription in this.subscriptions)
            {
                subscription(this.GetValue());
            }
        }
    }
}
