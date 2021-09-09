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
        public static IReadOnlyList<BindingDefinition> Definitions { get; }
        private static readonly Dictionary<BindingDefinition, BindingRuntimeData> definitions;

        public static event EventHandler<BindingErrorEventArgs> BindingError;

        static BindingsEngine()
        {
            definitions = new Dictionary<BindingDefinition, BindingRuntimeData>();
            Definitions = definitions.Keys.ToList().AsReadOnly();
        }

        public static void Attach(BindingDefinition binding)
        {
            if (binding is null)
            {
                throw new ArgumentNullException(nameof(binding));
            }

            var data = definitions[binding] = new BindingRuntimeData();

            if (binding.Mode == BindingMode.OneWay || binding.Mode == BindingMode.TwoWay)
            {
                binding.Source.Connect();
                data.SourceHandler = obj => PropagateValueToTarget(binding, obj);
                binding.Source.Subscribe(data.SourceHandler);
            }

            if (binding.Mode == BindingMode.OneWayToSource || binding.Mode == BindingMode.TwoWay)
            {
                binding.Target.Connect();
                data.TargetHandler = obj => PropagateValueToSource(binding, obj);
                binding.Target.Subscribe(data.TargetHandler);
            }

            if (binding.Mode == BindingMode.OneWayToSource)
            {
                PropagateValueToSource(binding, binding.Target.GetValue());
            }
            else
            {
                PropagateValueToTarget(binding, binding.Source.GetValue());
            }
        }

        public static void Detach(BindingDefinition binding)
        {
            if (!definitions.TryGetValue(binding, out var data))
                return;

            if (binding.Mode == BindingMode.OneWay || binding.Mode == BindingMode.TwoWay)
            {
                binding.Source.Unsubscribe(data.SourceHandler);
                binding.Source.Disconnect();
            }

            if (binding.Mode == BindingMode.OneWayToSource || binding.Mode == BindingMode.TwoWay)
            {
                binding.Target.Unsubscribe(data.TargetHandler);
                binding.Target.Disconnect();
            }
        }

        public static void DetachForObject(object target)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Definitions
                .Where(x => x.Source.IsDependentOn(target) || x.Target.IsDependentOn(target))
                .ToList() // evaluate to avoid modifying source collection while enumerating
                .ForEach(Detach);
        }

        private static void PropagateValueToTarget(BindingDefinition definition, object value)
        {
            try
            {
                value = definition.Convert(value);
            }
            catch (Exception e)
            {
                Error("Failed to convert value", definition, e);
            }

            try
            {
                definition.Target.SetValue(value);
            }
            catch (Exception e)
            {
                Error("Failed to set value to target", definition, e);
            }
        }

        private static void PropagateValueToSource(BindingDefinition definition, object value)
        {
            try
            {
                value = definition.ConvertBack(value);
            }
            catch (Exception e)
            {
                Error("Failed to convert value back", definition, e);
            }

            try
            {
                definition.Source.SetValue(value);
            }
            catch (Exception e)
            {
                Error("Failed to set value to source", definition, e);
            }
        }

        private static void Error(string message, BindingDefinition definition, Exception exception = null)
        {
            BindingError?.Invoke(null, new BindingErrorEventArgs(message, definition, exception));
        }

        private class BindingRuntimeData
        {
            public Action<object> SourceHandler;
            public Action<object> TargetHandler;
        }
    }
}
