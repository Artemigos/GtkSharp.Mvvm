using System;
using Gtk;
using GtkSharp.Mvvm.Bindings;

namespace GtkSharp.Mvvm.Sample
{
    public class Program
    {
        public static bool UseBindingLibrary => true;

        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();
            // BindingDefaults.NotificationTypeHandlers.Add(new GlibNotificationTypeHandler());
            // BindingDefaults.NotificationTypeHandlers.Add(new PropertyChangedNotificationTypeHandler());
            // BindingDefaults.NotificationTypeHandlers.Add(new CommandCanExecuteNotificationTypeHandler());
            // BindingDefaults.NotificationTypeHandlers.Add(new ErrorInfoChangedNotificationTypeHandler());

            var app = new Application("org.GtkSharp.Mvvm.Sample.GtkSharp.Mvvm.Sample", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
