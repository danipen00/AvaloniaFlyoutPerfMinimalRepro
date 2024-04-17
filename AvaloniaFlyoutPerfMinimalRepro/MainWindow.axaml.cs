using Avalonia.Controls;

namespace AvaloniaFlyoutPerfMinimalRepro;

public partial class MainWindow : Window, BranchesListPopupPanel.IOperations
{
    public MainWindow()
    {
        InitializeComponent();

        BranchesListPopupPanel popupPanel = new BranchesListPopupPanel(this);

        Flyout flyout = new ScrollLessFlyout();
        flyout.Placement = PlacementMode.BottomEdgeAlignedLeft;
        flyout.Content = popupPanel;

        Button button = new Button();
        button.Content = "Click me!";
        button.Flyout = flyout;

        StackPanel stackPanel = new StackPanel();
        stackPanel.Children.Add(button);
        this.Content = stackPanel;
    }

    public void SwitchToBranch(BranchInfo branchInfo)
    {

    }

    public void CreateNewBranch(string proposedBranchName)
    {

    }
}