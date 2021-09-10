using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GtkSharp.Mvvm.Observable;

namespace GtkSharp.Mvvm.Bindings.Attempt3
{
    public static class BindingExtensions
    {
        public static IObservable<TValue> ObservePath<TSource, TValue>(
            this TSource source,
            Expression<Func<TSource, TValue>> selector)
        {

            var currentExpr = selector.Body;
            var methods = new List<(ITrackingMethod method, Expression expr)>();

            do
            {
                var method = BindingSystem.TrackingMethods
                    .FirstOrDefault(x => x.CanTrack(currentExpr));
                if (method is null)
                {
                    throw new ArgumentException("Given selector is not supported by current tracking methods.", nameof(selector));
                }

                methods.Add((method, currentExpr));
                currentExpr = method.GetInnerExpression(currentExpr);
            }
            while (currentExpr is not ParameterExpression);

            methods.Reverse();
            var currentObservable = BuildTrackingLambda(currentExpr.Type, methods[0].expr, methods[0].method)
                .Compile().DynamicInvoke(source);
            currentExpr = methods[0].expr;

            foreach (var (method, expr) in methods.Skip(1))
            {
                var tracker = BuildTrackingLambda(currentExpr.Type, expr, method).Compile();
                currentObservable = typeof(ObservableExtensions)
                    .GetMethod(nameof(ObservableExtensions.SelectManyFromLatest))
                    .MakeGenericMethod(currentExpr.Type, expr.Type)
                    .Invoke(null, new object[] { currentObservable, tracker });
                currentExpr = expr;
            }

            return (IObservable<TValue>)currentObservable;
        }

        private static LambdaExpression BuildTrackingLambda(Type itemType, Expression getter, ITrackingMethod method)
        {
            // for generic types TSource, TResult, this creates a lambda equivalent to:
            // TSource x => ((IObservable<TResult>)method.Track(x, getter)).ResendLastOnSubscribe(x != null ? [getter(x)] : default)

            var resultType = getter.Type;
            ParameterExpression par = Expression.Parameter(itemType);
            var simpleGetter = method.CreateSimpleGet(getter, par);
            var currentValueGetter = Expression.Condition(
                Expression.NotEqual(par, Expression.Constant(null)),
                simpleGetter,
                Expression.Default(simpleGetter.Type)
            );
            return Expression.Lambda(
                Expression.Call(
                    null,
                    typeof(ObservableExtensions)
                        .GetMethod(nameof(ObservableExtensions.ResendLastOnSubscribe))
                        .MakeGenericMethod(resultType),
                    Expression.Convert(
                        Expression.Call(
                            Expression.Constant(method),
                            typeof(ITrackingMethod).GetMethod(nameof(method.Track)),
                            par,
                            Expression.Constant(getter)
                        ),
                        typeof(IObservable<>).MakeGenericType(resultType)
                    ),
                    currentValueGetter
                ),
                par
            );
        }
    }
}
