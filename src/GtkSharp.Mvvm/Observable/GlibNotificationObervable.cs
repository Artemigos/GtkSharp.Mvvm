using System;

namespace GtkSharp.Mvvm.Observable
{
    internal sealed class GlibNotificationObservable<TSource, TResult> : ObservableBase<TResult>
        where TSource : GLib.Object
    {
        private readonly TSource source;
        private readonly Func<TSource, TResult> getter;
        private readonly string glibPropertyName;

        public GlibNotificationObservable(TSource source, Func<TSource, TResult> getter, string glibPropertyName)
        {
            if (string.IsNullOrEmpty(glibPropertyName))
            {
                throw new ArgumentException($"'{nameof(glibPropertyName)}' cannot be null or empty.", nameof(glibPropertyName));
            }

            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.getter = getter ?? throw new ArgumentNullException(nameof(getter));
            this.glibPropertyName = glibPropertyName;
        }

        protected override void Attach()
        {
            source.AddNotification(glibPropertyName, HandleNotification);
        }

        protected override void Detach()
        {
            source.RemoveNotification(glibPropertyName, HandleNotification);
        }

        private void HandleNotification(object sender, GLib.NotifyArgs args)
        {
            if (args.Property == glibPropertyName)
            {
                PublishNext(getter(source));
            }
        }
    }
}
