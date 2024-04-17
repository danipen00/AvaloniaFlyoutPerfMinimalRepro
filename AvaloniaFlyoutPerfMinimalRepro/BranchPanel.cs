using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace AvaloniaFlyoutPerfMinimalRepro;

#nullable enable
internal class BranchPanel : DockPanel
{
    internal BranchPanel(
        Func<string> fetchFilterText,
        Func<BranchInfo?> fetchWorkingBranch,
        BranchesListPopupPanel.IOperations operations)
    {
        mFetchFilterText = fetchFilterText;
        mFetchWorkingBranch = fetchWorkingBranch;
        mOperations = operations;

        BuildComponents(
            out mImage,
            out mBranchNameTextBlock,
            out mBranchDescriptionTextBlock,
            out mDateTextBlock);
    }

    internal void UpdateBranchInfo(BranchInfo branchInfo)
    {
        mBranchInfo = branchInfo;
        Redraw(branchInfo);
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        Background = Brushes.Azure;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Background = Brushes.Beige;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        Background = IsPointerOver ?
            Brushes.Azure:
            Brushes.White;

        mOperations.SwitchToBranch(mBranchInfo);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        Background = Brushes.White;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is not BranchInfo branchInfo)
        {
            //UnsetEvents();
            return;
        }

        /*if (!mAreEventsSet)
            SetEvents();*/

        mBranchInfo = branchInfo;
        Redraw(branchInfo);
    }

    void Redraw(BranchInfo branchInfo)
    {
        string filterText = mFetchFilterText();

        SetBranchName(branchInfo.Name, filterText);
        SetBranchComment(branchInfo.Comment, filterText);

        bool isWorkingBranch = branchInfo.Equals(mFetchWorkingBranch());

        if (mIsWorkingBranch != isWorkingBranch)
            mImage.Source = GetBranchImage(isWorkingBranch);

        mIsWorkingBranch = isWorkingBranch;
        mDateTextBlock.Text = branchInfo.LocalTimeStamp.ToString();
        ToolTip.SetTip(mDateTextBlock, string.Format("{0}, {1}",
            branchInfo.LocalTimeStamp.ToLongDateString(),
            branchInfo.LocalTimeStamp.ToLongTimeString()));
    }

    void SetBranchComment(string branchInfoComment, string filterText)
    {
        mBranchDescriptionTextBlock.Inlines?.Clear();
        mBranchDescriptionTextBlock.Text = null;
        mBranchDescriptionTextBlock.FontStyle =
            string.IsNullOrEmpty(branchInfoComment) ? FontStyle.Italic : FontStyle.Normal;

        if (string.IsNullOrEmpty(filterText))
        {
            mBranchDescriptionTextBlock.Text = string.IsNullOrEmpty(branchInfoComment)
                ? "no comment"
                : branchInfoComment;
            return;
        }

        //SetInlines(mBranchDescriptionTextBlock, branchInfoComment, filterText);
    }

    void SetBranchName(string branchInfoName, string filterText)
    {
        mBranchNameTextBlock.Inlines?.Clear();
        mBranchNameTextBlock.Text = null;

        if (string.IsNullOrEmpty(filterText))
        {
            mBranchNameTextBlock.Text = branchInfoName;
            return;
        }

        //SetInlines(mBranchNameTextBlock, branchInfoName, filterText);
    }

    /*static void SetInlines(TextBlock target, string text, string filterText)
    {
        foreach (TextMatches.TextMatch match in
                 TextMatches.Get(text, filterText))
        {
            IBrush foreground = match.Matches
                ? ThemeBrushes.Name.HighlightActiveBrush.GetBrush()
                : ThemeBrushes.Name.TextBrush.GetBrush();

            FontWeight fontWeight = match.Matches ?
                FontWeight.DemiBold : FontWeight.Normal;

            target.Inlines?.Add(new Run(match.Text)
            {
                Foreground = foreground,
                FontWeight = fontWeight,
            });
        }
    }*/

    static IImage GetBranchImage(bool isWorkingBranch)
    {
        /*IImage branchIcon = PlasticImages.Name.IconBranch.GetImage();

        return isWorkingBranch ?
            PlasticImages.GetCurrentBottomRightIcon(branchIcon) :
            branchIcon;*/
        return null;
    }

    void BuildComponents(
        out Image image,
        out TextBlock branchNameTextBlock,
        out TextBlock branchDescriptionTextBlock,
        out TextBlock dateTextBlock)
    {
        Background = Brushes.White;

        image = new Image() { Width = ICON_SIZE, Height = ICON_SIZE };
        image.VerticalAlignment = VerticalAlignment.Center;

        branchNameTextBlock = new TextBlockAutoTooltip();
        branchNameTextBlock.TextTrimming = TextTrimming.CharacterEllipsis;
        branchNameTextBlock.VerticalAlignment = VerticalAlignment.Center;

        dateTextBlock = new TextBlock();
        dateTextBlock.VerticalAlignment = VerticalAlignment.Center;
        dateTextBlock.Margin = new Thickness(0, 0, 5, 0);

        branchDescriptionTextBlock = new TextBlockAutoTooltip();
        branchDescriptionTextBlock.TextTrimming = TextTrimming.CharacterEllipsis;

        branchNameTextBlock.Margin = new Thickness(0, 0, 5, 0);

        DockPanel branchNamePanel = new DockPanel();
        branchNamePanel.AddChildren(
            left: image,
            fill: branchNameTextBlock);

        StackPanel branchNameAndDescriptionPanel = new StackPanel();
        branchNameAndDescriptionPanel.Spacing = 3;
        branchNameAndDescriptionPanel.Children.Add(branchNamePanel);
        branchNameAndDescriptionPanel.Children.Add(branchDescriptionTextBlock);

        branchNameAndDescriptionPanel.Margin = new Thickness(
            10,
            5);

        this.AddChildren(
            fill: branchNameAndDescriptionPanel,
            right: dateTextBlock);
    }

    const double ICON_SIZE = 16;

    readonly Func<string> mFetchFilterText;
    readonly Func<BranchInfo?> mFetchWorkingBranch;
    readonly TextBlock mBranchNameTextBlock;
    readonly TextBlock mBranchDescriptionTextBlock;
    readonly TextBlock mDateTextBlock;
    readonly Image mImage;
    readonly BranchesListPopupPanel.IOperations mOperations;

    BranchInfo mBranchInfo;
    bool mIsWorkingBranch;
}

#nullable disable