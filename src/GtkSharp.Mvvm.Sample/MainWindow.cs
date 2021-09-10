using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Gtk;

namespace GtkSharp.Mvvm.Sample
{
    class MainWindow : Window
    {
        private MainViewModel viewModel = new MainViewModel();

        private Button plus = null;
        private Button minus = null;
        private Label counterLabel = null;
        private Entry validatedEntry = null;
        private Label entryRepeatLabel = null;
        private Label errorInfoLabel = null;

        public MainWindow()
            : base("Example")
        {
            this.DefaultWidth = 480;
            this.DefaultHeight = 240;
            this.Titlebar = this.CreateTitlebar();
            this.Add(this.CreateContent());
            this.ShowAll();

            if (Program.UseBindingLibrary)
            {
                #region BINDINGS IN USER CODE

                viewModel.ObservePath(x => x.Text)
                    .Subscribe(val => counterLabel.Text = val)
                    .AttachToWidgetLifetime(this);

                viewModel.ObservePath(x => x.Entry).Subscribe(val =>
                {
                    validatedEntry.Text = val;
                    entryRepeatLabel.Text = val;
                }).AttachToWidgetLifetime(this);

                validatedEntry.ObservePath(x => x.Text)
                    .Subscribe(val => viewModel.Entry = val)
                    .AttachToWidgetLifetime(this);

                plus.BindCommand(viewModel, x => x.IncrementCounter);
                minus.BindCommand(viewModel, x => x.DecrementCounter);

                viewModel.ObservePath(x => x.GetErrors(nameof(viewModel.Entry)))
                    .Subscribe(val => errorInfoLabel.Markup = "<span foreground='red'>" + string.Join("\n", val.Cast<string>()) + "</span>")
                    .AttachToWidgetLifetime(this);

                #endregion
            }
            else
            {
                #region SHOULD BE DONE BY THE ENGINE

                this.viewModel.PropertyChanged += this.HandleViewModelPropertyChanged;
                this.viewModel.IncrementCounter.CanExecuteChanged += this.HandleIncrementCounterCanExecuteChanged;
                this.viewModel.DecrementCounter.CanExecuteChanged += this.HandleDecrementCounterCanExecuteChanged;
                this.validatedEntry.AddNotification("text", this.HandleLabel1Notification);
                this.plus.Clicked += this.Button1_Clicked;
                this.minus.Clicked += this.Button2_Clicked;
                this.viewModel.ErrorsChanged += this.HandleViewModelErrorsChanged;

                this.counterLabel.Text = viewModel.Text;
                this.validatedEntry.Text = viewModel.Entry;
                this.entryRepeatLabel.Text = viewModel.Entry;
                this.plus.Sensitive = viewModel.IncrementCounter.CanExecute(null);
                this.minus.Sensitive = viewModel.DecrementCounter.CanExecute(null);

                #endregion
            }

            DeleteEvent += Window_DeleteEvent;
        }

        #region SHOULD BE DONE BY THE ENGINE

        private void HandleViewModelErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.Entry))
            {
                var val = this.viewModel.GetErrors(nameof(viewModel.Entry));
                Func<IEnumerable, object> converter = (errs => "<span foreground='red'>" + string.Join("\n", errs.Cast<string>()) + "</span>");
                this.errorInfoLabel.Markup = (string)converter(val);
            }
        }

        private void HandleLabel1Notification(object o, GLib.NotifyArgs args)
        {
            if (args.Property == "text")
            {
                this.viewModel.Entry = this.validatedEntry.Text;
            }
        }

        private void HandleIncrementCounterCanExecuteChanged(object sender, EventArgs e)
        {
            this.plus.Sensitive = viewModel.IncrementCounter.CanExecute(null);
        }

        private void HandleDecrementCounterCanExecuteChanged(object sender, EventArgs e)
        {
            this.minus.Sensitive = viewModel.DecrementCounter.CanExecute(null);
        }

        private void HandleViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.Text))
            {
                var val = viewModel.Text;
                this.counterLabel.Text = val;
            }
            else if (e.PropertyName == nameof(viewModel.Entry))
            {
                var val = viewModel.Entry;
                this.validatedEntry.Text = val;
                this.entryRepeatLabel.Text = val;
            }
        }

        private void Button1_Clicked(object sender, EventArgs a)
        {
            var command = this.viewModel?.IncrementCounter;
            if (command?.CanExecute(null) ?? false)
                command.Execute(null);
        }

        private void Button2_Clicked(object sender, EventArgs e)
        {
            var command = this.viewModel?.DecrementCounter;
            if (command?.CanExecute(null) ?? false)
                command.Execute(null);
        }

        #endregion

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private Box CreateContent()
        {
            var content = new Box(Orientation.Vertical, 0) { Margin = 4 };
            this.counterLabel = new Label("");
            this.validatedEntry = new Entry();
            this.entryRepeatLabel = new Label("[empty]");
            this.errorInfoLabel = new Label("");

            content.PackStart(this.counterLabel, false, false, 0);
            content.PackStart(this.validatedEntry, false, false, 0);
            content.PackStart(this.entryRepeatLabel, false, false, 0);
            content.PackStart(this.errorInfoLabel, false, false, 0);

            return content;
        }

        private Widget CreateTitlebar()
        {
            var header = new HeaderBar
            {
                Title = "Example",
                Subtitle = "for GtkSharp.Mvvm testing",
            };

            this.plus = new Button("+");
            this.minus = new Button("-");
            header.PackStart(plus);
            header.PackStart(minus);

            return header;
        }
    }
}
