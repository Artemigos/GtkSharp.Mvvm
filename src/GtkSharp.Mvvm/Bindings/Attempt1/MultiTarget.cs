using System;

namespace GtkSharp.Mvvm.Bindings
{
    public class MultiTarget : IBindingTarget
    {
        private readonly IBindingTarget[] targets;

        private readonly Delegate mergeFunction;

        public MultiTarget(IBindingTarget[] targets, Delegate mergeFunction)
        {
            this.targets = targets ?? throw new ArgumentNullException(nameof(targets));
            this.mergeFunction = mergeFunction ?? throw new ArgumentNullException(nameof(mergeFunction));

            if (targets.Length < 2)
            {
                throw new ArgumentException("At least 2 targets required to create a multi-target.");
            }
        }

        public bool CanTrack => throw new NotImplementedException();

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public object GetValue()
        {
            throw new NotImplementedException();
        }

        public bool IsDependentOn(object dependency)
        {
            throw new NotImplementedException();
        }

        public void SetValue(object value)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Action<object> handler)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(Action<object> handler)
        {
            throw new NotImplementedException();
        }
    }
}

