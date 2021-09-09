using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GtkSharp.Mvvm.Bindings
{
    public abstract class TrackingMethodBase : ITrackingMethod
    {
        public abstract bool CanTrack(Expression expression);
        public abstract Expression GetInnerExpression(Expression expression);

        public IObservable<object> Track(object source, Expression expression)
        {
            var tSource = source.GetType();
            var tResult = expression.Type;

            var typedMethod = this.GetType().GetMethod("TrackTyped", BindingFlags.NonPublic | BindingFlags.Instance);
            var observable = typedMethod.MakeGenericMethod(tSource, tResult).Invoke(this, new[] { source, expression });

            return (IObservable<object>)observable;
        }
    }
}
