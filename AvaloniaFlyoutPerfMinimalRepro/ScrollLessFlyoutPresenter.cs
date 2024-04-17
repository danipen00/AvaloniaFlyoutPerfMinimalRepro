using Avalonia;
using Avalonia.Controls;

namespace AvaloniaFlyoutPerfMinimalRepro;

public class ScrollLessFlyout : Flyout
{
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<ScrollLessFlyout, CornerRadius>(nameof(CornerRadius), new CornerRadius(4));

    public CornerRadius CornerRadius
    {
        get { return GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    protected override Control CreatePresenter()
    {
        FlyoutPresenter result = new ScrollLessFlyoutPresenter()
        {
            [!ContentControl.ContentProperty] = this[!ContentProperty]
        };
        result.CornerRadius = CornerRadius;
        return result;
    }
}

public class ScrollLessFlyoutPresenter : FlyoutPresenter { }