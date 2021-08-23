using System;
using Gtk;
using GtkSharp.Mvvm.Bindings;
using UI = Gtk.Builder.ObjectAttribute;

namespace GtkSharp.Mvvm
{
    class MainWindow : Window
    {
        private MainViewModel viewModel = new MainViewModel();

        [UI] private Label _label1 = null;
        [UI] private Button _button1 = null;

        private int _counter;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;
            _button1.Clicked += Button1_Clicked;

            _label1.Bind(x => x.Text).To(viewModel, x => x.Text);
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void Button1_Clicked(object sender, EventArgs a)
        {
            viewModel.Counter++;
            viewModel.Text = "Hello World! This button has been clicked " + _counter + " time(s).";
        }
    }
}
