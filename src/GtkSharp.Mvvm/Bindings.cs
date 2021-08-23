using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace GtkTest
{
    /// <summary>
    /// The binding engine.
    /// </summary>
    public static class Bindings
    {
        public static IReadOnlyList<BindingDefinition> Definitions => definitions.AsReadOnly();
        private static List<BindingDefinition> definitions = new List<BindingDefinition>();

        public static event EventHandler<BindingErrorEventArgs> BindingError;

        public static void Attach(BindingDefinition binding)
        {
            throw new NotImplementedException();
        }

        public static void Detach(BindingDefinition binding)
        {
            throw new NotImplementedException();
        }

        public static void DetachForObject(object target)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            definitions
                .Where(x => x.Source.IsDependentOn(target) || x.Target.IsDependentOn(target))
                .ToList() // evaluate to avoid modifying source collection while enumerating
                .ForEach(Detach);
        }
    }

    /// <summary>
    /// Fluent binding API.
    /// </summary>
    public static class BindingExtensions
    {
        public static BindingDefinition To(
            this IBindingTarget target,
            IBindingTarget source,
            BindingMode mode = default,
            BindingConverter convert = null,
            BindingConverter convertBack = null)
        {
            convert ??= (convertBack == null ? (x => x) : (_ => throw new NotSupportedException()));
            convertBack ??= (convert == null ? (x => x) : (_ => throw new NotSupportedException()));
            var definition = new BindingDefinition(source, target, mode, convert, convertBack);
            Bindings.Attach(definition);
            return definition;
        }

        public static BindingDefinition To(
            this IBindingTarget target,
            INotifyPropertyChanged source,
            string propertyName,
            BindingMode mode = default,
            BindingConverter convert = null,
            BindingConverter convertBack = null)
        {
            return target.To(source.Bind(propertyName));
        }

        public static BindingDefinition To<TSource, TValue>(
            this IBindingTarget target,
            TSource source,
            Expression<Func<TSource, TValue>> selector,
            BindingMode mode = default,
            BindingConverter convert = null,
            BindingConverter convertBack = null)
            where TSource : INotifyPropertyChanged
        {
            return target.To(source.Bind(selector));
        }

        public static IBindingTarget Bind(this INotifyPropertyChanged target, string propertyName)
        {
            return new NotifyBindingTarget(target, propertyName);
        }

        public static IBindingTarget Bind<TTarget, TValue>(this TTarget target, Expression<Func<TTarget, TValue>> selector)
            where TTarget : INotifyPropertyChanged
        {
            var name = GetPropertyName(selector);
            return target.Bind(name);
        }

        public static string GetPropertyName<TTarget, TValue>(Expression<Func<TTarget, TValue>> selector)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// GTK fluent binding API.
    /// </summary>
    public static class GtkBindingExtensions
    {
        public static BindingDefinition To(
            this IBindingTarget target,
            GLib.Object source,
            string propertyName,
            BindingMode mode = default,
            BindingConverter convert = null,
            BindingConverter convertBack = null)
        {
            return target.To(source.Bind(propertyName));
        }

        public static BindingDefinition To<TSource, TValue>(
            this IBindingTarget target,
            TSource source,
            Expression<Func<TSource, TValue>> selector,
            BindingMode mode = default,
            BindingConverter convert = null,
            BindingConverter convertBack = null)
            where TSource : GLib.Object
        {
            return target.To(source.Bind(selector));
        }

        public static IBindingTarget Bind(this GLib.Object target, string propertyName)
        {
            return new GObjectBindingTarget(target, propertyName);
        }

        public static IBindingTarget Bind<TTarget, TValue>(this TTarget target, Expression<Func<TTarget, TValue>> selector)
            where TTarget : GLib.Object
        {
            var name = BindingExtensions.GetPropertyName(selector);
            return target.Bind(name);
        }
    }

    public delegate object BindingConverter(object input);

    public interface IBindingTarget
    {
        void Connect();
        void Disconnect();
        object GetValue();
        void SetValue(object value);
        void Subscribe(Action<object> handler);
        void Unsubscribe(Action<object> handler);
        bool IsDependentOn(object dependency);
    }

    public enum BindingMode
    {
        OneWay,
        OneWayToSource,
        TwoWay
    }

    public class BindingDefinition
    {
        public BindingDefinition(
            IBindingTarget source,
            IBindingTarget target,
            BindingMode mode,
            BindingConverter convert,
            BindingConverter convertBack)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Mode = mode;
            Convert = convert ?? throw new ArgumentNullException(nameof(convert));
            ConvertBack = convertBack ?? throw new ArgumentNullException(nameof(convertBack));
        }

        public IBindingTarget Source { get; }
        public IBindingTarget Target { get; }
        public BindingMode Mode { get; }
        public BindingConverter Convert { get; }
        public BindingConverter ConvertBack { get; }
    }

    public class NotifyBindingTarget : IBindingTarget
    {
        public NotifyBindingTarget(INotifyPropertyChanged target, string propertyName)
        {
            throw new NotImplementedException();
        }

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

    public class GObjectBindingTarget : IBindingTarget
    {
        public GObjectBindingTarget(GLib.Object target, string propertyName)
        {
            throw new NotImplementedException();
        }

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
