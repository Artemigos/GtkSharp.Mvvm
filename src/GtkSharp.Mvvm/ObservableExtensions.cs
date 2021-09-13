using System;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;
using GtkSharp.Mvvm.Observable;

namespace GtkSharp.Mvvm
{
    public static class ObservableExtensions
    {
        public static IDisposable Subscribe<TItem>(
            this IObservable<TItem> observable,
            Action<TItem> onNext,
            Action<Exception> onError = null,
            Action onCompleted = null)
        {
            if (observable is null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            if (onNext is null)
            {
                throw new ArgumentNullException(nameof(onNext));
            }

            return observable.Subscribe(new DelegatedObserver<TItem>(onNext, onError, onCompleted));
        }

        public static IObservable<TValue> ObserveProperty<TSource, TValue>(
            this TSource source,
            Expression<Func<TSource, TValue>> selector)
            where TSource : INotifyPropertyChanged
        {
            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (!(selector.Body is MemberExpression mem) || !(mem.Member is PropertyInfo prop))
            {
                throw new ArgumentException("Only property selectors are supported.", nameof(selector));
            }

            if (!prop.CanRead)
            {
                throw new ArgumentException("The property does not have a getter.", nameof(selector));
            }

            if (!(mem.Expression is ParameterExpression))
            {
                throw new ArgumentException("The property should be selected directly from the parameter.", nameof(selector));
            }

            var selectorFun = selector.Compile();
            return new PropertyChangeObservable<TSource, TValue>(source, selectorFun, prop.Name);
        }

        public static IObservable<bool> ObserveCanExecute(this ICommand command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return new CanExecuteChangedObservable(command);
        }

        public static IObservable<IEnumerable> ObserveErrors<TSource, TValue>(
            this TSource source,
            Expression<Func<TSource, TValue>> selector)
            where TSource : INotifyDataErrorInfo
        {
            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (!(selector.Body is MemberExpression mem) || !(mem.Member is PropertyInfo prop))
            {
                throw new ArgumentException("Only property selectors are supported.", nameof(selector));
            }

            if (!prop.CanRead)
            {
                throw new ArgumentException("The property does not have a getter.", nameof(selector));
            }

            if (!(mem.Expression is ParameterExpression))
            {
                throw new ArgumentException("The property should be selected directly from the parameter.", nameof(selector));
            }

            return new ErrorsChangedObservable<TSource>(source, prop.Name);
        }

        public static IObservable<TValue> ObserveNotifications<TSource, TValue>(
            this TSource source,
            Expression<Func<TSource, TValue>> selector)
            where TSource : GLib.Object
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (!(selector.Body is MemberExpression mem) || !(mem.Member is PropertyInfo prop))
            {
                throw new ArgumentException("Only property selectors are supported.", nameof(selector));
            }

            if (!prop.CanRead)
            {
                throw new ArgumentException("The property does not have a getter.", nameof(selector));
            }

            if (!(mem.Expression is ParameterExpression))
            {
                throw new ArgumentException("The property should be selected directly from the parameter.", nameof(selector));
            }

            var attr = prop.GetCustomAttribute<GLib.PropertyAttribute>();
            if (attr is null)
            {
                throw new ArgumentException("The property does not support notifications.", nameof(selector));
            }

            var selectorFun = selector.Compile();
            return new GlibNotificationObservable<TSource, TValue>(source, selectorFun, attr.Name);
        }

        public static IObservable<TResult> SelectManyFromLatest<TSource, TResult>(
            this IObservable<TSource> source,
            Func<TSource, IObservable<TResult>> selector)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return new SelectManyFromLatestObservable<TSource, TResult>(source, selector);
        }

        public static IValueObservable<T> ResendLastOnSubscribe<T>(this IObservable<T> source, T initialValue = default)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new ResendLastOnSubscribeObservable<T>(source, initialValue);
        }
    }
}

