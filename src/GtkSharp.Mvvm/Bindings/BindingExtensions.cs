using System;
using System.Linq;
using System.Linq.Expressions;
using GtkSharp.Mvvm.Observable;

namespace GtkSharp.Mvvm.Bindings.Attempt3
{
    public static class BindingExtensions
    {
        public static IObservable<TValue> Bind<TSource, TValue>(this TSource source, Expression<Func<TSource, TValue>> selector)
        {
            var method = FindTrackingMethod(selector.Body);
            if (method is null)
            {
                throw new ArgumentException("Given selector is not supported by current tracking methods.", nameof(selector));
            }

            var observable = method.Track(source, selector.Body);
            return ((IObservable<TValue>)observable).ResendLastOnSubscribe(selector.Compile().Invoke(source));
        }

        private static ITrackingMethod FindTrackingMethod(Expression selector)
        {
            return BindingSystem.TrackingMethods.FirstOrDefault(x => x.CanTrack(selector));
        }
    }
}
