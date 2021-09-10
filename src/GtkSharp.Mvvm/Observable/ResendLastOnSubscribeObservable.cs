using System;

namespace GtkSharp.Mvvm.Observable
{
    internal sealed class ResendLastOnSubscribeObservable<TResult> : ObservableBase<TResult>, IObserver<TResult>, IValueObservable<TResult>
    {
        private readonly IObservable<TResult> source;
        private TResult lastResult;
        private IDisposable subscription;

        public TResult CurrentValue => lastResult;

        public ResendLastOnSubscribeObservable(IObservable<TResult> source, TResult initialValue)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.lastResult = initialValue;
        }

        public void OnCompleted()
        {
            PublishCompleted();
        }

        public void OnError(Exception error)
        {
            PublishError(error);
        }

        public void OnNext(TResult value)
        {
            lastResult = value;
            PublishNext(value);
        }

        protected override void InitializeObserver(IObserver<TResult> observer)
        {
            observer.OnNext(lastResult);
            base.InitializeObserver(observer);
        }

        protected override void Attach()
        {
            source.Subscribe(this);
        }

        protected override void Detach()
        {
            subscription?.Dispose();
            subscription = null;
        }
    }
}
