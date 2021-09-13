using System;
using System.Linq.Expressions;
using System.Reflection;
using GtkSharp.Mvvm.Observable;

namespace GtkSharp.Mvvm.Bindings
{
    public class GlibNotificationTrackingMethod : TrackingMethodBase
    {
        public override bool CanTrack(Expression expression)
        {
            if (expression is null)
            {
                return false;
            }

            if (!(expression is MemberExpression mem) || !(mem.Member is PropertyInfo prop))
            {
                return false;
            }

            if (!prop.CanRead)
            {
                return false;
            }

            if (!typeof(GLib.Object).IsAssignableFrom(mem.Expression.Type))
            {
                return false;
            }

            var attr = prop.GetCustomAttribute<GLib.PropertyAttribute>();
            if (attr is null)
            {
                return false;
            }

            return true;
        }

        public override Expression CreateSimpleGet(Expression expression, Expression getFrom)
        {
            var mem = (MemberExpression)expression;
            if (mem.Expression.Type != getFrom.Type)
            {
                throw new ArgumentException("The type of the given source does not math original type.");
            }

            return mem.Update(getFrom);
        }

        public override Expression GetInnerExpression(Expression expression)
        {
            var mem = (MemberExpression)expression;
            return mem.Expression;
        }

        private IObservable<TResult> TrackTyped<TSource, TResult>(TSource source, Expression expression)
            where TSource : GLib.Object
        {
            var mem = (MemberExpression)expression;
            var prop = (PropertyInfo)mem.Member;
            var attr = prop.GetCustomAttribute<GLib.PropertyAttribute>();
            var par = Expression.Parameter(typeof(TSource));
            var newMem = Expression.Property(par, prop);
            var lambda = Expression.Lambda<Func<TSource, TResult>>(newMem, par).Compile();

            return new GlibNotificationObservable<TSource,TResult>(source, lambda, attr.Name);
        }
    }
}
