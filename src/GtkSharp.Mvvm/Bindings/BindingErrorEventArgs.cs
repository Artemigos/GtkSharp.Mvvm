using System;

namespace GtkSharp.Mvvm.Bindings
{
    public class BindingErrorEventArgs : EventArgs
    {
        public BindingErrorEventArgs(string message, Exception exception = null)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Exception = exception;
        }

        public string Message { get; }
        public Exception Exception { get; }
    }
}
