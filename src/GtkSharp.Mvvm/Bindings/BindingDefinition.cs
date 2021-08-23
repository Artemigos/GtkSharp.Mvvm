using System;

namespace GtkSharp.Mvvm.Bindings
{
    public class BindingDefinition
    {
        public BindingDefinition(
            IBindingTarget source,
            IBindingTarget target,
            BindingMode mode,
            BindingConverter convert,
            BindingConverter convertBack)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Mode = mode;
            Convert = convert ?? throw new ArgumentNullException(nameof(convert));
            ConvertBack = convertBack ?? throw new ArgumentNullException(nameof(convertBack));

            if ((mode == BindingMode.TwoWay || mode == BindingMode.OneWay) && !source.CanTrack)
            {
                throw new ArgumentException($"Cannot bind with binding mode {mode} when source can't be tracked");
            }

            if ((mode == BindingMode.TwoWay || mode == BindingMode.OneWayToSource) && !target.CanTrack)
            {
                throw new ArgumentException($"Cannot bind with binding mode {mode} when target can't be tracked");
            }
        }

        public IBindingTarget Source { get; }
        public IBindingTarget Target { get; }
        public BindingMode Mode { get; }
        public BindingConverter Convert { get; }
        public BindingConverter ConvertBack { get; }
    }
}
