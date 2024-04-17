using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AvaloniaFlyoutPerfMinimalRepro;


    public class BranchInfo
    {
        public string Name { get; set; }
        public Guid GUID { get; set; }
        public string Comment { get; set; }
        public DateTime LocalTimeStamp { get; set; }
    }

    public static class PlasticLocalization
    {
        public enum Name
        {
            NewBranchButton
        }

        public static string GetString(PlasticLocalization.Name name)
        {
            return name.ToString();
        }
    }

    public class TextBlockAutoTooltip : TextBlock
    {
        protected override TextLayout CreateTextLayout(string text)
        {
            TextLayout result = base.CreateTextLayout(text);
            try
            {
                SetupTooltipVisibility(result);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        void SetupTooltipVisibility(TextLayout layout)
        {
            if (this.TextTrimming == TextTrimming.None)
                return;

            bool showTooltip = !string.IsNullOrEmpty(this.Text ?? this.Inlines?.Text) &&
                               layout.TextLines.Any(line => line.HasCollapsed);

            if (showTooltip)
                ToolTip.SetTip(this, this.Text ?? this.Inlines?.Text);
            else
                ToolTip.SetTip(this, null);
        }
    }

    public enum DockPanelChildrenOrder
    {
        TopBottomLeftRight,
        TopBottomRightLeft,
        TopLeftRightBottom,
        LeftRightTopBottom,
    }
    public static class DockPanelExtensions
    {
        public static void AddChildren(
            this DockPanel panel,
            Control top = null,
            Control left = null,
            Control fill = null,
            Control right = null,
            Control bottom = null,
            DockPanelChildrenOrder order = DockPanelChildrenOrder.TopBottomLeftRight)
        {
            AddChildren(
                panel,
                top: top != null ? new []{ top } : null,
                left: left != null ? new []{ left } : null,
                fill: fill,
                right: right != null ? new []{ right } : null,
                bottom: bottom != null ? new []{ bottom } : null,
                order: order);
        }

        public static void AddChildren<T>(
             this DockPanel panel,
             T[] top = null,
             T[] left = null,
             T fill = null,
             T[] right = null,
             T[] bottom = null,
             DockPanelChildrenOrder order = DockPanelChildrenOrder.TopBottomLeftRight)
            where T: Control
        {
            // Dockpanel navigation mode needs to be updated in order to manage tabulation locally.
            // TabIndexes are considered on local subtree only inside this container.
            KeyboardNavigation.SetTabNavigation(
                panel, KeyboardNavigationMode.Local);

            int tabIndex = 0;
            int bottomTabIndex = (top?.Length ?? 0) +
                (left?.Length ?? 0) +
                (right?.Length ?? 0) +
                                 (fill == null ? 0 : 1);

            switch (order)
            {
                case DockPanelChildrenOrder.TopBottomLeftRight:
                    panel.AddChildrenToTop(top, ref tabIndex);
                    panel.AddChildrenToBottom(bottom, bottomTabIndex);
                    panel.AddChildrenToLeft(left, ref tabIndex);
                    panel.AddChildrenToRight(right, ref tabIndex);
                    break;

                case DockPanelChildrenOrder.TopBottomRightLeft:
                    panel.AddChildrenToTop(top, ref tabIndex);
                    panel.AddChildrenToBottom(bottom, bottomTabIndex);
                    panel.AddChildrenToRight(right, ref tabIndex);
                    panel.AddChildrenToLeft(left, ref tabIndex);
                    break;

                case DockPanelChildrenOrder.TopLeftRightBottom:
                    panel.AddChildrenToTop(top, ref tabIndex);
                    panel.AddChildrenToLeft(left, ref tabIndex);
                    panel.AddChildrenToRight(right, ref tabIndex);
                    panel.AddChildrenToBottom(bottom, bottomTabIndex);
                    break;

                case DockPanelChildrenOrder.LeftRightTopBottom:
                    panel.AddChildrenToLeft(left, ref tabIndex);
                    panel.AddChildrenToRight(right, ref tabIndex);
                    panel.AddChildrenToTop(top, ref tabIndex);
                    panel.AddChildrenToBottom(bottom, bottomTabIndex);
                    break;
            }

            panel.AddChildToCenter(fill, (top?.Length ?? 0) + (left?.Length ?? 0));
        }

        static void AddChildrenToRight(
            this DockPanel panel,
            Control[] children,
            ref int tabIndex)
        {
            if (children == null)
                return;

            int index = children.Length + tabIndex;
            tabIndex = index;

            for (int i = children.Length - 1; i >= 0; i--)
            {
                Control element = children[i];

                if (element == null)
                    continue;

                KeyboardNavigation.SetTabNavigation(
                    element, KeyboardNavigationMode.Local);

                DockPanel.SetDock(element, Dock.Right);

                element.TabIndex = index--;

                panel.Children.Add(element);
            }
        }

        static void AddChildToCenter(
            this DockPanel panel,
            Control fillElement,
            int tabIndex)
        {
            if (fillElement == null)
            {
                panel.LastChildFill = false;
                return;
            }

            KeyboardNavigation.SetTabNavigation(
                fillElement, KeyboardNavigationMode.Local);

            fillElement.TabIndex = tabIndex;

            panel.Children.Add(fillElement);
        }

        static void AddChildrenToLeft(
            this DockPanel panel,
            Control[] children,
            ref int tabIndex)
        {
            if (children == null)
                return;

            foreach (Control control in children)
            {
                if (control == null)
                    continue;

                KeyboardNavigation.SetTabNavigation(
                    control, KeyboardNavigationMode.Local);

                DockPanel.SetDock(control, Dock.Left);

                control.TabIndex = tabIndex++;

                panel.Children.Add(control);
            }
        }

        static void AddChildrenToTop(
            this DockPanel panel,
            Control[] children,
            ref int tabIndex)
        {
            if (children == null)
                return;

            foreach (Control control in children)
            {
                if (control == null)
                    continue;

                KeyboardNavigation.SetTabNavigation(
                    control, KeyboardNavigationMode.Local);

                DockPanel.SetDock(control, Dock.Top);

                control.TabIndex = tabIndex++;

                panel.Children.Add(control);
            }
        }

        static void AddChildrenToBottom(
            this DockPanel panel,
            Control[] children,
            int tabIndex)
        {
            if (children == null)
                return;

            foreach (Control control in children)
            {
                if (control == null)
                    continue;

                /*KeyboardNavigation.SetTabNavigation(
                    control, KeyboardNavigationMode.Local);*/

                DockPanel.SetDock(control, Dock.Bottom);

                control.TabIndex = tabIndex++;

                panel.Children.Add(control);
            }
        }
    }
