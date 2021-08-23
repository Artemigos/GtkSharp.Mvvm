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
        [UI] private Label _label2 = null;
        [UI] private Button _button1 = null;
        [UI] private Entry _textBox1 = null;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;
            _button1.Clicked += Button1_Clicked;

            _label1.Bind(x => x.Text).To(viewModel, x => x.Text);
            _textBox1.Bind(x => x.Text).To(viewModel, x => x.Entry, BindingMode.TwoWay);
            _label2.Bind(x => x.Text).To(viewModel, x => x.Entry);
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void Button1_Clicked(object sender, EventArgs a)
        {
            viewModel.Counter++;
        }
    }
}
