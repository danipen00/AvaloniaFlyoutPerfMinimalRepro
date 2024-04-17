using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaFlyoutPerfMinimalRepro;

#nullable enable
internal class SectionPanel : Panel
{
    internal SectionPanel()
    {
        Margin = new Thickness(
            10,
            5);

        BuildComponents(out mSectionNameTextBlock);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is not SectionInfo section)
            return;

        mSectionNameTextBlock.Text = section.SectionName;
    }

    void BuildComponents(out TextBlock sectionNameTextBlock)
    {
        sectionNameTextBlock = new TextBlock();
        sectionNameTextBlock.FontWeight = FontWeight.DemiBold;

        Children.Add(sectionNameTextBlock);
    }

    readonly TextBlock mSectionNameTextBlock;
}
#nullable disable