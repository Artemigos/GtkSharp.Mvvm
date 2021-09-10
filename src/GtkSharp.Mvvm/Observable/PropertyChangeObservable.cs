using System;
using System.ComponentModel;

namespace GtkSharp.Mvvm.Observable
{
    internal sealed class PropertyChangeObservable<TSource, TResult> : ObservableBase<TResult>
        where TSource : INotifyPropertyChanged
    {
        private readonly TSource source;
        private readonly Func<TSource, TResult> getter;
        private readonly string propertyName;

        public PropertyChangeObservable(TSource source, Func<TSource, TResult> getter, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or empty.", nameof(propertyName));
            }

            this.source = source;
            this.getter = getter ?? throw new ArgumentNullException(nameof(getter));
            this.propertyName = propertyName;
        }

        protected override void Attach()
        {
            source.PropertyChanged += HandlePropertyChanged;
        }

        protected override void Detach()
        {
            source.PropertyChanged -= HandlePropertyChanged;
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == propertyName)
            {
                PublishNext(getter(source));
            }
        }
    }
}
