using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace GtkSharp.Mvvm.Bindings
{
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
            BindingsEngine.Attach(definition);
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
            if (selector.Body is MemberExpression mem)
            {
                if (mem.Member is PropertyInfo prop)
                {
                    return prop.Name;
                }

                throw new NotSupportedException("Only property members are supported.");
            }

            throw new NotSupportedException("Only direct member expressions are supported.");
        }
    }
}
