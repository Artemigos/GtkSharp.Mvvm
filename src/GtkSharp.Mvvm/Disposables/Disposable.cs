using System;
using System.Threading;

namespace GtkSharp.Mvvm.Disposables
{
    internal class Disposable : IDisposable
    {
        private Action dispose;

        public Disposable(Action dispose) =>
            this.dispose = dispose ?? throw new ArgumentNullException(nameof(dispose));

        public void Dispose()
        {
            var dispose = Interlocked.Exchange(ref this.dispose, null);
            dispose?.Invoke();
        }
    }
}

