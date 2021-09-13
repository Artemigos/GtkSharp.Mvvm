using System;
using System.Collections.Generic;
using GtkSharp.Mvvm.Disposables;

namespace GtkSharp.Mvvm.Observable
{
    internal abstract class ObservableBase<TResult> : IObservable<TResult>
    {
        private readonly object lck = new object();
        private readonly List<IObserver<TResult>> observers = new List<IObserver<TResult>>();

        public IDisposable Subscribe(IObserver<TResult> observer)
        {
            lock (this.lck)
            {
                this.InitializeObserver(observer);
                this.observers.Add(observer);
                if (this.observers.Count == 1)
                {
                    this.Attach();
                }
            }

            return new Disposable(() =>
            {
                lock (this.lck)
                {
                    if (this.observers.Remove(observer) && this.observers.Count == 0)
                    {
                        this.Detach();
                    }
                }
            });
        }

        protected virtual void InitializeObserver(IObserver<TResult> observer) { }
        protected abstract void Attach();
        protected abstract void Detach();

        protected void PublishCompleted()
        {
            IObserver<TResult>[] obs;
            lock (this.lck)
            {
                obs = observers.ToArray();
            }

            foreach (var o in obs)
            {
                o.OnCompleted();
            }
        }

        protected void PublishNext(TResult result)
        {
            IObserver<TResult>[] obs;
            lock (this.lck)
            {
                obs = observers.ToArray();
            }

            foreach (var o in obs)
            {
                o.OnNext(result);
            }
        }

        protected void PublishError(Exception error)
        {
            IObserver<TResult>[] obs;
            lock (this.lck)
            {
                obs = observers.ToArray();
            }

            foreach (var o in obs)
            {
                o.OnError(error);
            }
        }
    }
}
