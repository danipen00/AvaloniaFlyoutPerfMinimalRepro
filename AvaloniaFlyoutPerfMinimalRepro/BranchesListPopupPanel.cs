using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AvaloniaFlyoutPerfMinimalRepro;

#nullable enable

internal class BranchesListPopupPanel : DockPanel/*IFilterableTable, IRefreshableView, IDisposable*/
{
    internal interface IOperations
    {
        void SwitchToBranch(BranchInfo branchInfo);
        void CreateNewBranch(string proposedBranchName);
    }

    internal BranchesListPopupPanel(
        IOperations operations)
    {
        Width = 450;
        MinHeight = 250;

        mOperations = operations;

        BuildComponents(
            operations,
            out mProgressBar,
            out mFilterEntry,
            out mCreateBranchButton,
            out mBranchesList,
            out mListContainerPanel);

        List<object> itemsSource = new List<object>();

        itemsSource.Add(new SectionInfo()
        {
            SectionName = "Main branch"
        });
        itemsSource.Add(new BranchInfo()
        {
            Name = "/main",
            Comment = "Main branch",
            LocalTimeStamp = DateTime.Now,
        });

        itemsSource.Add(new SectionInfo()
        {
            SectionName = "Feature branches"
        });
        itemsSource.Add(new BranchInfo()
        {
            Name = "/main/feature1",
            Comment = "Feature 1",
            LocalTimeStamp = DateTime.Now,
        });
        itemsSource.Add(new BranchInfo()
        {
            Name = "/main/feature2",
            Comment = "Feature 2",
            LocalTimeStamp = DateTime.Now,
        });

        itemsSource.Add(new SectionInfo()
        {
            SectionName = "Other branches"
        });

        for (int i = 0; i < 21; i++)
        {
            itemsSource.Add(new BranchInfo()
            {
                Name = "/main/bugfix" + i.ToString(),
                Comment = "Bugfix " + i.ToString(),
                LocalTimeStamp = DateTime.Now,
            });
        }


        SetItemsSource(itemsSource);
    }


    void SetItemsSource(IEnumerable? itemsSource)
    {
        //Vector scrollOffset = mScrollViewer.Offset;
        mBranchesList.ItemsSource = null;
        mBranchesList.ItemsSource = itemsSource;
        //mScrollViewer.Offset = scrollOffset;

        SetupEmptyStateVisibility();
    }

    void SetupEmptyStateVisibility()
    {
        /*if (mBranchesListModel.IsEmpty)
        {
            ShowEmptyStatePanel();
            return;
        }

        HideEmptyStatePanel();*/
    }

    void ShowEmptyStatePanel()
    {
        /*if (mListContainerPanel.Children[0] == mEmptyStatePanel)
            return;

        mListContainerPanel.Children.Clear();
        mListContainerPanel.AddChildren(fill: mEmptyStatePanel);*/
    }

    void HideEmptyStatePanel()
    {
        if (mListContainerPanel.Children[0] == mBranchesList)
            return;

        mListContainerPanel.Children.Clear();
        mListContainerPanel.AddChildren(fill: mBranchesList);
    }

    void ItemContainerGenerator_ContainerClearing(object? sender, ContainerClearingEventArgs e)
    {
        if (e.Container is not ContentPresenter)
            return;

        /*if (e.Container.DataContext is not TModel)
            return;

        FilterListItem<TModel>? listItem =
            e.Container.FindDescendantOfType<FilterListItem<TModel>>();
        listItem?.UnsetEvents();*/
    }

    void ItemContainerGenerator_ContainerPrepared(object? sender, ContainerPreparedEventArgs e)
    {
        if (e.Container is not ContentPresenter container)
            return;

        if (e.Container.DataContext is not BranchInfo)
            return;

        container.Focusable = false;

        if (container.Padding != default)
            container.Padding = default;

        if (e.Container.DataContext is not BranchInfo branchInfo)
            return;

        BranchPanel? branchPanel =
            container.FindDescendantOfType<BranchPanel>();
        branchPanel?.UpdateBranchInfo(branchInfo);
    }

    void CreateBranchButton_Click(object? sender, RoutedEventArgs e)
    {
        mOperations.CreateNewBranch(
            mFilterEntry.Text);
    }

    void BuildComponents(
        IOperations operations,
        out ProgressBar progressBar,
        out TextBox filterEntry,
        out Button createBranchButton,
        out ItemsControl branchesList,
        out DockPanel listContainerPanel)
    {
        Panel progressBarPanel = CreateProgressBarPanel(out progressBar);

        Panel createBranchPanel = BuildCreateBranchPanel(
            out filterEntry,
            out createBranchButton);

        Func<string> fetchFilterText = () => mFilterEntry.Text;

        listContainerPanel = new DockPanel();
        branchesList = new ListBox();
        branchesList.ItemsPanel = new FuncTemplate<Panel?>(() => new VirtualizingStackPanel());
        branchesList.DataTemplates.Add(new FuncDataTemplate<BranchInfo>((_, _) =>
            new BranchPanel(
                fetchFilterText,
                () => mWorkingBranch,
                operations),
            true));

        branchesList.DataTemplates.Add(new FuncDataTemplate<SectionInfo>((_, _) =>
            new SectionPanel(), true));

        /*EventExceptionsCatcher.SetContainerPreparedEvent(
            branchesList,
            ItemContainerGenerator_ContainerPrepared,
            mEventsTranslator);
        EventExceptionsCatcher.SetContainerClearingEvent(
            branchesList,
            ItemContainerGenerator_ContainerClearing,
            mEventsTranslator);*/

        listContainerPanel.AddChildren(fill: branchesList);

        this.AddChildren(
            top: new[] { progressBarPanel, createBranchPanel },
            fill: listContainerPanel);
    }

    Panel CreateProgressBarPanel(out ProgressBar progressBar)
    {
        progressBar = new ProgressBar();
        mProgressBar.IsIndeterminate = true;
        mProgressBar.IsVisible = true;
        mProgressBar.MinHeight = 5;
        mProgressBar.Height = 5;
        mProgressBar.VerticalAlignment = VerticalAlignment.Top;
        mProgressBar.HorizontalAlignment = HorizontalAlignment.Stretch;

        DockPanel result = new DockPanel();
        result.Height = 5;
        result.AddChildren(fill: progressBar);
        return result;
    }

    Panel BuildCreateBranchPanel(
        out TextBox filterEntry,
        out Button createBranchButton)
    {
        filterEntry = new TextBox();
        //filterEntry.EnableDynamicWidth();

        createBranchButton = new Button();
        createBranchButton.Content = PlasticLocalization.Name.NewBranchButton.ToString();
        createBranchButton.Margin = new Thickness(5, 0, 0, 0);



        DockPanel result = new DockPanel();

        result.Margin = new Thickness(
            10,
            5);

        result.AddChildren(
            fill: mFilterEntry,
            right: createBranchButton);
        return result;
    }

    readonly IOperations mOperations;
    readonly ProgressBar mProgressBar;
    readonly TextBox mFilterEntry;
    readonly Button mCreateBranchButton;
    readonly ItemsControl mBranchesList;
    readonly DockPanel mListContainerPanel;

    BranchInfo? mWorkingBranch;
}

#nullable disable