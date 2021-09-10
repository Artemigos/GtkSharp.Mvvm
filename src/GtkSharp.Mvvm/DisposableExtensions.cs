using System;
using Gtk;

namespace GtkSharp.Mvvm
{
    public static class DisposableExtensions
    {
        public static IDisposable AttachToWidgetLifetime(this IDisposable disposable, Widget widget)
        {
            widget.Destroyed += (sender, args) => disposable.Dispose();
            return disposable;
        }
    }
}

