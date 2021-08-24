using System;

namespace GtkSharp.Mvvm.Bindings
{
    public class BindingErrorEventArgs : EventArgs
    {
        public BindingErrorEventArgs(string message, BindingDefinition definition, Exception exception = null)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Definition = definition;
            Exception = exception;
        }

        public string Message { get; }
        public Exception Exception { get; }
        public BindingDefinition Definition { get; }
    }
}
