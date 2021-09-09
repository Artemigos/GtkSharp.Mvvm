using System;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;

namespace GtkSharp.Mvvm.Bindings
{
    public static class ErrorInfoExtensions
    {
        public static IBindingTarget BindErrors(this INotifyDataErrorInfo target, string propertyName)
        {
            return new ErrorsBindingTarget(target, propertyName);
        }

        public static IBindingTarget BindErrors<TTarget, TValue>(this TTarget target, Expression<Func<TTarget, TValue>> selector)
            where TTarget : INotifyDataErrorInfo
        {
            var name = BindingExtensions.GetPropertyName(selector);
            return target.BindErrors(name);
        }

        public static BindingDefinition ToErrors(
                this IBindingTarget target,
                IBindingTarget source,
                Func<IEnumerable, object> convert = null)
        {
            convert ??= (x => x);
            BindingConverter actualConvert = (x => convert((IEnumerable)x));
            var definition = new BindingDefinition(source, target, BindingMode.OneWay, actualConvert, x => x);
            BindingsEngine.Attach(definition);
            return definition;
        }

        public static BindingDefinition ToErrors(
                this IBindingTarget target,
                INotifyDataErrorInfo source,
                string propertyName,
                Func<IEnumerable, object> convert = null)
        {
            return target.ToErrors(source.BindErrors(propertyName), convert);
        }

        public static BindingDefinition ToErrors<TSource, TValue>(
                this IBindingTarget target,
                TSource source,
                Expression<Func<TSource, TValue>> selector,
                Func<IEnumerable, object> convert = null)
            where TSource : INotifyDataErrorInfo
        {
            return target.ToErrors(source.BindErrors(selector), convert);
        }
    }
}
