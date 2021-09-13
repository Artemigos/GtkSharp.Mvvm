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
            if (implementation == @interface)
            {
                return method;
            }

            var map = implementation.GetInterfaceMap(@interface);
            for (int i = 0; i < map.TargetMethods.Length; ++i)
            {
                var comparedMethod = map.TargetMethods[i];
                if (comparedMethod.ReflectedType != method.ReflectedType)
                {
                    // This is relevant when the method is implemented on a base type and not overriden.
                    // The `GetInterfaceMap` call finds it through the `implementation` type, while
                    // the expressions give the method directly for the base type that has the method.
                    // As a result method comparison fails, even though in practice they are the same methods.
                    comparedMethod = comparedMethod.GetBaseDefinition();
                }

                if (method == comparedMethod)
                {
                    return map.InterfaceMethods[i];
                }
            }

            return null;
        }
    }
}
