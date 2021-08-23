using System;
using System.Linq.Expressions;

namespace GtkSharp.Mvvm.Bindings
{
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
}
