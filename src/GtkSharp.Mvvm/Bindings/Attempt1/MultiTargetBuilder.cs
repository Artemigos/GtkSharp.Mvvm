using System;
using System.Collections.Generic;

namespace GtkSharp.Mvvm.Bindings
{
    public class MultiTargetBuilder
    {
        protected IBindingTarget target;
        protected List<IBindingTarget> sources;
        protected Delegate mergingFunction;

        public MultiTargetBuilder(IBindingTarget target)
        {
            this.target = target;
            this.sources = new List<IBindingTarget>();
            this.mergingFunction = null;
        }

        public virtual BindingDefinition Bind()
        {
            throw new InvalidOperationException("Cannot bind with less that 2 sources defined.");
        }
    }
/*
    public class MultiTargetBuilder<T1> : MultiTargetBuilder
    {
    }

    public class MultiTargetBuilder<T1, T2> : MultiTargetBuilder
    {
        public override BindingDefinition Bind()
        {
            var source = new MultiTarget(this.sources.ToArray(), this.mergingFunction);
            return new BindingDefinition(source, target, BindingMode.OneWay, x => x, x => x);
        }
    }

    public class MultiTargetBuilder<T1, T2, T3> : MultiTargetBuilder<T1, T2>
    {
    }

    public class MultiTargetBuilder<T1, T2, T3, T4> : MultiTargetBuilder<T1, T2>
    {
    }

    public class MultiTargetBuilder<T1, T2, T3, T4, T5> : MultiTargetBuilder<T1, T2>
    {
    }

    public class MultiTargetBuilder<T1, T2, T3, T4, T5, T6> : MultiTargetBuilder<T1, T2>
    {
    }

    public class MultiTargetBuilder<T1, T2, T3, T4, T5, T6, T7> : MultiTargetBuilder<T1, T2>
    {
    }

    public class MultiTargetBuilder<T1, T2, T3, T4, T5, T6, T7, T8> : MultiTargetBuilder<T1, T2>
    {
    }
*/
}

