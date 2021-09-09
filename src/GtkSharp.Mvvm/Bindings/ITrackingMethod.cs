using System;
using System.Linq.Expressions;

namespace GtkSharp.Mvvm.Bindings
{
    public interface ITrackingMethod
    {
        bool CanTrack(Expression expression);
        Expression GetInnerExpression(Expression expression);
        IObservable<object> Track(object source, Expression expression);
    }
}
