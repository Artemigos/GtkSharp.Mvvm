using System;
using System.Linq.Expressions;

namespace GtkSharp.Mvvm.Bindings
{
    public interface ITrackingMethod
    {
        bool CanTrack(Expression expression);
        Expression GetInnerExpression(Expression expression);
        Expression CreateSimpleGet(Expression expression, Expression getFrom);
        object Track(object source, Expression expression);
    }
}
