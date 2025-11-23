using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    /// <summary>
    /// Context interface providing state and settings to sidebar painters
    /// </summary>
    public interface ISideBarPainterContext
    {
        // Drawing surface
        Graphics Graphics { get; }
        Rectangle Bounds { get; }
        Rectangle DrawingRect { get; }

        // MenuStyle and colors
        IBeepTheme Theme { get; }
        bool UseThemeColors { get; }
        Color AccentColor { get; }
        Color BackColor { get; }

        // Menu items
        BindingList<SimpleItem> Items { get; }
        SimpleItem SelectedItem { get; }
        SimpleItem HoveredItem { get; }
        Dictionary<SimpleItem, bool> ExpandedState { get; }

        // State
        bool IsCollapsed { get; }
        bool IsAnimating { get; }
        int ItemHeight { get; }
        int ChildItemHeight { get; }

        // Layout
        int ExpandedWidth { get; }
        int CollapsedWidth { get; }
        int IndentationWidth { get; }
        int ChromeCornerRadius { get; }
        bool EnableRailShadow { get; }

        // Interaction
        bool IsEnabled { get; }
        bool ShowToggleButton { get; }
        // Default icon path to use when item has no ImagePath set
        string DefaultImagePath { get; }
        // Current control style (allows painters to adjust fallback icons per style)
        TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle ControlStyle { get; }
    }
}
