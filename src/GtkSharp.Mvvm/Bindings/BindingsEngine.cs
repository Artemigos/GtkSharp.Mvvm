using System;
using System.Collections.Generic;
using System.Linq;

namespace GtkSharp.Mvvm.Bindings
{
    /// <summary>
    /// The binding engine.
    /// </summary>
    public static class BindingsEngine
    {
        public static IReadOnlyList<BindingDefinition> Definitions => definitions.AsReadOnly();
        private static List<BindingDefinition> definitions = new List<BindingDefinition>();

        public static event EventHandler<BindingErrorEventArgs> BindingError;

        public static void Attach(BindingDefinition binding)
        {
            throw new NotImplementedException();
        }

        public static void Detach(BindingDefinition binding)
        {
            throw new NotImplementedException();
        }

        public static void DetachForObject(object target)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            definitions
                .Where(x => x.Source.IsDependentOn(target) || x.Target.IsDependentOn(target))
                .ToList() // evaluate to avoid modifying source collection while enumerating
                .ForEach(Detach);
        }
    }
}
