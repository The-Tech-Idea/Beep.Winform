using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    public enum BeepTabHeaderActionKind
    {
        None,
        SelectTab,
        CloseTab,
        AddTab,
        Overflow,
        CloseCurrent,
        ScrollBackward,
        ScrollForward
    }

    /// <summary>
    /// Represents a resolved header action from the custom tab header host.
    /// The model stays intentionally small so the premium host can route tab and
    /// action commands without leaking owner-control implementation details.
    /// </summary>
    public sealed class BeepTabHeaderAction
    {
        public static BeepTabHeaderAction CreateActionSlot(
            BeepTabHeaderActionKind actionKind,
            string commandName,
            string displayText,
            int order,
            bool isVisible = true,
            bool isEnabled = true)
        {
            return new BeepTabHeaderAction
            {
                ActionKind = actionKind,
                CommandName = commandName,
                DisplayText = displayText,
                Order = order,
                IsActionSlot = true,
                IsVisible = isVisible,
                IsEnabled = isEnabled
            };
        }

        public static BeepTabHeaderAction None(Point location)
        {
            return new BeepTabHeaderAction
            {
                ActionKind = BeepTabHeaderActionKind.None,
                CommandName = string.Empty,
                TabIndex = -1,
                HitLocation = location,
                IsVisible = false,
                IsEnabled = false
            };
        }

        public static BeepTabHeaderAction SelectTab(int tabIndex, Point location)
        {
            return new BeepTabHeaderAction
            {
                ActionKind = BeepTabHeaderActionKind.SelectTab,
                CommandName = "tab.select",
                TabIndex = tabIndex,
                HitLocation = location,
                IsVisible = true,
                IsEnabled = true
            };
        }

        public static BeepTabHeaderAction CloseTab(int tabIndex, Point location)
        {
            return new BeepTabHeaderAction
            {
                ActionKind = BeepTabHeaderActionKind.CloseTab,
                CommandName = "tab.close",
                TabIndex = tabIndex,
                HitLocation = location,
                IsVisible = true,
                IsEnabled = true
            };
        }

        public BeepTabHeaderActionKind ActionKind { get; init; }
        public string CommandName { get; init; } = string.Empty;
        public string DisplayText { get; init; } = string.Empty;
        public int Order { get; init; }
        public int TabIndex { get; init; } = -1;
        public Point HitLocation { get; init; }
        public bool IsVisible { get; init; }
        public bool IsEnabled { get; init; }
        public bool IsActionSlot { get; init; }
        public Rectangle Bounds { get; set; } = Rectangle.Empty;
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }

        public bool HitTest(Point location)
        {
            return IsVisible && !Bounds.IsEmpty && Bounds.Contains(location);
        }
    }
}