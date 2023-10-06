using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace GeneA.CustomControls
{
    public class PopupBase : TemplatedControl
    {
        public static readonly StyledProperty<Control> PopupContentProperty =
        AvaloniaProperty.Register<PopupBase, Control>(nameof(PopupContent));

        public object PopupContent
        {
            get { return GetValue(PopupContentProperty); }
            set { SetValue(PopupContentProperty, value); }
        }
    }
}
