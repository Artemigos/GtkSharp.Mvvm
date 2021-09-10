using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GtkSharp.Mvvm.Bindings
{
    public abstract class TrackingMethodBase : ITrackingMethod
    {
        public abstract bool CanTrack(Expression expression);
        public abstract Expression CreateSimpleGet(Expression expression, Expression getFrom);
        public abstract Expression GetInnerExpression(Expression expression);

        public object Track(object source, Expression expression)
        {
            var tSource = source.GetType();
            var tResult = expression.Type;

            var typedMethod = this.GetType().GetMethod("TrackTyped", BindingFlags.NonPublic | BindingFlags.Instance);
            var observable = typedMethod.MakeGenericMethod(tSource, tResult).Invoke(this, new[] { source, expression });

            return observable;
        }

        protected MethodInfo FindMethodOnInterface(Type implementation, Type @interface, MethodInfo method)
        {
            var map = implementation.GetInterfaceMap(@interface);
            for (int i = 0; i < map.TargetMethods.Length; ++i)
            {
                if (method.HasSameMetadataDefinitionAs(map.TargetMethods[i]))
                {
                    return map.InterfaceMethods[i];
                }
            }

            return null;
        }
    }
}
