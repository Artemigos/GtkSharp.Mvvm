using System;

namespace GtkSharp.Mvvm.Observable
{
    internal sealed class SelectManyFromLatestObservable<TSource, TResult> : ObservableBase<TResult>, IObserver<TSource>
    {
        private readonly IObservable<TSource> source;
        private IDisposable sourceSubscription;
        private readonly Func<TSource, IObservable<TResult>> selector;
        private IObservable<TResult> lastObservable;
        private IDisposable lastObservableSubscription;
        private readonly object lck = new object();
        private readonly _ forwarder;

        public SelectManyFromLatestObservable(IObservable<TSource> source, Func<TSource, IObservable<TResult>> selector)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.selector = selector ?? throw new ArgumentNullException(nameof(selector));
            this.forwarder = new _(this);
        }

        protected override void Attach()
        {
            this.sourceSubscription = this.source.Subscribe(this);
        }

        protected override void Detach()
        {
            this.sourceSubscription.Dispose();
            this.sourceSubscription = null;
        }

        public void OnCompleted()
        {
            this.PublishCompleted();
        }

        public void OnError(Exception error)
        {
            this.PublishError(error);
        }

        public void OnNext(TSource value)
        {
            lock (this.lck)
            {
                this.lastObservable = this.selector(value);
                this.lastObservableSubscription?.Dispose();
                this.lastObservableSubscription = this.lastObservable?.Subscribe(forwarder);
            }
        }

        private sealed class _ : IObserver<TResult>
        {
            private readonly SelectManyFromLatestObservable<TSource, TResult> owner;

            public _(SelectManyFromLatestObservable<TSource, TResult> owner)
            {
                this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
                owner.PublishError(error);
            }

            public void OnNext(TResult value)
            {
                owner.PublishNext(value);
            }
        }
    }
}
