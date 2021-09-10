using System.Collections;
using System.ComponentModel;

namespace GtkSharp.Mvvm.Observable
{
    internal sealed class ErrorsChangedObservable<TSource> : ObservableBase<IEnumerable>
        where TSource : INotifyDataErrorInfo
    {
        private readonly TSource source;
        private readonly string propertyName;

        public ErrorsChangedObservable(TSource source, string propertyName)
        {
            this.source = source;
            this.propertyName = propertyName ?? throw new System.ArgumentNullException(nameof(propertyName));
        }

        protected override void Attach()
        {
            this.source.ErrorsChanged += HandleErrorsChanged;
        }

        protected override void Detach()
        {
            this.source.ErrorsChanged -= HandleErrorsChanged;
        }

        private void HandleErrorsChanged(object sender, DataErrorsChangedEventArgs args)
        {
            if (args.PropertyName == propertyName)
            {
                PublishNext(source.GetErrors(propertyName));
            }
        }
    }
}
