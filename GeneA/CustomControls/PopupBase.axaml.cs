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

        public static readonly StyledProperty<double> PopupHeightProperty =
       AvaloniaProperty.Register<PopupBase, double>(nameof(PopupHeight));

        public double PopupHeight
        {
            get { return GetValue(PopupHeightProperty); }
            set { SetValue(PopupHeightProperty, value); }
        }

        public static readonly StyledProperty<double> PopupWidthProperty =
       AvaloniaProperty.Register<PopupBase, double>(nameof(PopupWidth));

        public double PopupWidth
        {
            get { return GetValue(PopupWidthProperty); }
            set { SetValue(PopupWidthProperty, value); }
        }
    }
}
