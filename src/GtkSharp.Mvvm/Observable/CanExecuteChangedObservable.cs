using System;
using System.Windows.Input;

namespace GtkSharp.Mvvm.Observable
{
    internal sealed class CanExecuteChangedObservable : ObservableBase<bool>
    {
        private readonly ICommand command;

        public CanExecuteChangedObservable(ICommand command)
        {
            this.command = command ?? throw new System.ArgumentNullException(nameof(command));
        }

        protected override void Attach()
        {
            command.CanExecuteChanged += HandleCanExecuteChanged;
        }

        protected override void Detach()
        {
            command.CanExecuteChanged -= HandleCanExecuteChanged;
        }

        private void HandleCanExecuteChanged(object sender, EventArgs args)
        {
            PublishNext(command.CanExecute(null));
        }
    }
}
