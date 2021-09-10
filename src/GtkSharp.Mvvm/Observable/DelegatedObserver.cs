using System;

namespace GtkSharp.Mvvm.Observable
{
    internal sealed class DelegatedObserver<TItem> : IObserver<TItem>
    {
        private readonly Action<TItem> onNext;
        private readonly Action<Exception> onError;
        private readonly Action onCompleted;

        public DelegatedObserver(
            Action<TItem> onNext,
            Action<Exception> onError = null,
            Action onCompleted = null)
        {
            this.onNext = onNext ?? throw new ArgumentNullException(nameof(onNext));
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        public void OnCompleted()
        {
            onCompleted?.Invoke();
        }

        public void OnError(Exception error)
        {
            onError?.Invoke(error);
        }

        public void OnNext(TItem value)
        {
            onNext(value);
        }
    }
}