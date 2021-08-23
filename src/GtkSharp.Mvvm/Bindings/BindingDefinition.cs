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
        }

        public IBindingTarget Source { get; }
        public IBindingTarget Target { get; }
        public BindingMode Mode { get; }
        public BindingConverter Convert { get; }
        public BindingConverter ConvertBack { get; }
    }
}
