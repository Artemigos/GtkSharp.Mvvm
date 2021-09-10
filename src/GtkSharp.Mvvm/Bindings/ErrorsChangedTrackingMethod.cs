using System;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using GtkSharp.Mvvm.Observable;

namespace GtkSharp.Mvvm.Bindings
{
    public class ErrorsChangedTrackingMethod : TrackingMethodBase
    {
        public override bool CanTrack(Expression expression)
        {
            if (expression is null)
            {
                return false;
            }

            if (expression is not MethodCallExpression mem)
            {
                return false;
            }

            if (mem.Object == null || !typeof(INotifyDataErrorInfo).IsAssignableFrom(mem.Object.Type))
            {
                return false;
            }

            var methodOrigin = this.FindMethodOnInterface(mem.Object.Type, typeof(INotifyDataErrorInfo), mem.Method);
            if (methodOrigin != typeof(INotifyDataErrorInfo).GetMethod(nameof(INotifyDataErrorInfo.GetErrors)))
            {
                return false;
            }

            var nameArg = mem.Arguments[0];
            if (nameArg is not ConstantExpression)
            {
                return false;
            }

            return true;
        }

        public override Expression CreateSimpleGet(Expression expression, Expression getFrom)
        {
            var mem = (MethodCallExpression)expression;
            if (mem.Object.Type != getFrom.Type)
            {
                throw new ArgumentException("The type of the given source does not math original type.");
            }

            return mem.Update(getFrom, mem.Arguments);
        }

        public override Expression GetInnerExpression(Expression expression)
        {
            var mem = (MethodCallExpression)expression;
            return mem.Object;
        }

        private IObservable<IEnumerable> TrackTyped<TSource, _>(TSource source, Expression expression)
            where TSource : INotifyDataErrorInfo
        {
            var mem = (MethodCallExpression)expression;
            var arg = (ConstantExpression)mem.Arguments[0];

            return new ErrorsChangedObservable<TSource>(source, (string)arg.Value);
        }
    }
}
