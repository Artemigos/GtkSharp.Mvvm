using System;
using System.Linq.Expressions;
using System.Windows.Input;
using GtkSharp.Mvvm.Observable;

namespace GtkSharp.Mvvm.Bindings
{
    public class CanExecuteChangedTrackingMethod : TrackingMethodBase
    {
        public override bool CanTrack(Expression expression)
        {
            if (expression is null)
            {
                return false;
            }

            if (expression is not MethodCallExpression  mem)
            {
                return false;
            }

            if (mem.Object == null || !typeof(ICommand).IsAssignableFrom(mem.Object.Type))
            {
                return false;
            }

            var methodOrigin = this.FindMethodOnInterface(mem.Object.Type, typeof(ICommand), mem.Method);
            if (methodOrigin != typeof(ICommand).GetMethod(nameof(ICommand.CanExecute)))
            {
                return false;
            }

            var nameArg = mem.Arguments[0];
            if (nameArg is not ConstantExpression { Value: null })
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

        private IObservable<bool> TrackTyped<TSource, TResult>(TSource source, Expression expression)
            where TSource : ICommand
        {
            return new CanExecuteChangedObservable(source);
        }
    }
}
