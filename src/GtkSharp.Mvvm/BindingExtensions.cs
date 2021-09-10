using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;
using Gtk;
using GtkSharp.Mvvm.Bindings;
using GtkSharp.Mvvm.Disposables;
using GtkSharp.Mvvm.Observable;

namespace GtkSharp.Mvvm
{
    public static class BindingExtensions
    {
        public static IDisposable BindCommand<TSource>(
            this Button button,
            TSource source,
            Expression<Func<TSource, ICommand>> commandSelector)
        {
            var commandObervable = source.ObservePath(commandSelector);
            var canExecuteBinding = commandObervable
                .ObserveInnerProperty(x => x.CanExecute(null))
                .Subscribe(val => button.Sensitive = val);

            EventHandler handler = (sender, args) =>
            {
                var cmd = commandObervable.CurrentValue;
                if (cmd.CanExecute(null))
                {
                    cmd.Execute(null);
                }
            };

            button.Clicked += handler;
            return new Disposable(() =>
            {
                canExecuteBinding.Dispose();
                button.Clicked -= handler;
            }).AttachToWidgetLifetime(button);
        }

        public static IValueObservable<TResult> ObserveInnerProperty<TSource, TResult>(
            this IObservable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            ITrackingMethod method = null)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            method ??= FindTrackingMethod(selector.Body);
            return source
                .SelectManyFromLatest(x => PropertyObserverSelector<TSource, TResult>(x, selector, method))
                .ResendLastOnSubscribe(default);
        }

        private static IValueObservable<TResult> PropertyObserverSelector<TSource, TResult>(
            TSource source,
            Expression<Func<TSource, TResult>> selector,
            ITrackingMethod method)
        {
            return ((IObservable<TResult>)method.Track(source, selector.Body)).ResendLastOnSubscribe(selector.Compile().Invoke(source));
        }

        public static IValueObservable<TValue> ObservePath<TSource, TValue>(
            this TSource source,
            Expression<Func<TSource, TValue>> selector)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var currentExpr = selector.Body;
            var methods = new List<(ITrackingMethod method, Expression expr)>();

            do
            {
                var method = FindTrackingMethod(currentExpr);
                if (method is null)
                {
                    throw new ArgumentException("Given selector is not supported by current tracking methods.", nameof(selector));
                }

                methods.Add((method, currentExpr));
                currentExpr = method.GetInnerExpression(currentExpr);
            }
            while (currentExpr is not ParameterExpression);

            methods.Reverse();
            var wrapped = Expression.Lambda(methods[0].expr, (ParameterExpression)currentExpr);
            var currentObservable = typeof(BindingExtensions)
                .GetMethod(nameof(BindingExtensions.PropertyObserverSelector), BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(currentExpr.Type, methods[0].expr.Type)
                .Invoke(null, new object[] { source, wrapped, methods[0].method });
            currentExpr = methods[0].expr;

            foreach (var (method, expr) in methods.Skip(1))
            {
                var par = Expression.Parameter(currentExpr.Type);
                var swap = method.CreateSimpleGet(expr, par);
                wrapped = Expression.Lambda(swap, par);
                currentObservable = typeof(BindingExtensions)
                    .GetMethod(nameof(BindingExtensions.ObserveInnerProperty))
                    .MakeGenericMethod(currentExpr.Type, expr.Type)
                    .Invoke(null, new object[] { currentObservable, wrapped, method });
                currentExpr = expr;
            }

            TValue currentValue;
            try { currentValue = selector.Compile().Invoke(source); }
            catch (Exception) { currentValue = default; }
            return ((IObservable<TValue>)currentObservable).ResendLastOnSubscribe(currentValue);
        }

        private static ITrackingMethod FindTrackingMethod(Expression currentExpr)
        {
            return BindingSystem.TrackingMethods.FirstOrDefault(x => x.CanTrack(currentExpr));
        }
    }
}
