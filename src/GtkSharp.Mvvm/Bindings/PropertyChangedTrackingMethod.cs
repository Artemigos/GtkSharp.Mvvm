using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using GtkSharp.Mvvm.Observable;

namespace GtkSharp.Mvvm.Bindings
{
    public class PropertyChangedTrackingMethod : TrackingMethodBase
    {
        public override bool CanTrack(Expression expression)
        {
            if (expression is null)
            {
                return false;
            }

            if (expression is not MemberExpression { Member: PropertyInfo prop } mem)
            {
                return false;
            }

            if (!prop.CanRead)
            {
                return false;
            }

            if (!typeof(INotifyPropertyChanged).IsAssignableFrom(mem.Expression.Type))
            {
                return false;
            }

            return true;
        }

        public override Expression GetInnerExpression(Expression expression)
        {
            var mem = (MemberExpression)expression;
            return mem.Expression;
        }

        private IObservable<TResult> TrackTyped<TSource, TResult>(TSource source, Expression expression)
            where TSource : INotifyPropertyChanged
        {
            var mem = (MemberExpression)expression;
            var prop = (PropertyInfo)mem.Member;
            var par = Expression.Parameter(typeof(TSource));
            var newMem = Expression.Property(par, prop);
            var lambda = Expression.Lambda<Func<TSource, TResult>>(newMem, par).Compile();

            return new PropertyChangeObservable<TSource, TResult>(source, lambda, prop.Name);
        }
    }
}
