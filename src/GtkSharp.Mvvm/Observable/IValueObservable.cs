using System;

namespace GtkSharp.Mvvm.Observable
{
    public interface IValueObservable<TItem> : IObservable<TItem>
    {
        TItem CurrentValue { get; }
    }
}
