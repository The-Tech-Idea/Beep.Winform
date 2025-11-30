using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    /// <summary>
    /// Context interface providing state and settings to sidebar painters.
    /// Provides all information needed for painters to render distinct visual styles.
    /// </summary>
    public interface ISideBarPainterContext
    {
        #region Drawing Surface
        /// <summary>Graphics object for painting</summary>
        Graphics Graphics { get; }
        /// <summary>Full control bounds</summary>
        Rectangle Bounds { get; }
        /// <summary>Drawing rectangle (may exclude borders)</summary>
        Rectangle DrawingRect { get; }
        #endregion

        #region Theme and Colors
        /// <summary>Current theme name</summary>
        string ThemeName { get; }
        /// <summary>Current theme object</summary>
        IBeepTheme Theme { get; }
        /// <summary>Whether to use theme colors vs custom colors</summary>
        bool UseThemeColors { get; }
        /// <summary>Accent color for highlights</summary>
        Color AccentColor { get; }
        /// <summary>Background color</summary>
        Color BackColor { get; }
        #endregion

        #region Menu Items
        /// <summary>List of top-level menu items</summary>
        BindingList<SimpleItem> Items { get; }
        /// <summary>Currently selected item</summary>
        SimpleItem SelectedItem { get; }
        /// <summary>Currently hovered item</summary>
        SimpleItem HoveredItem { get; }
        /// <summary>Currently pressed item (mouse down)</summary>
        SimpleItem PressedItem { get; }
        /// <summary>Expansion state for accordion items</summary>
        Dictionary<SimpleItem, bool> ExpandedState { get; }
        #endregion

        #region State
        /// <summary>Whether sidebar is in collapsed (icon-only) mode</summary>
        bool IsCollapsed { get; }
        /// <summary>Whether collapse/expand animation is in progress</summary>
        bool IsAnimating { get; }
        /// <summary>Height of top-level menu items</summary>
        int ItemHeight { get; }
        /// <summary>Height of child menu items</summary>
        int ChildItemHeight { get; }
        #endregion

        #region Layout
        /// <summary>Width when expanded</summary>
        int ExpandedWidth { get; }
        /// <summary>Width when collapsed</summary>
        int CollapsedWidth { get; }
        /// <summary>Indentation width for child items</summary>
        int IndentationWidth { get; }
        /// <summary>Corner radius for rounded elements</summary>
        int ChromeCornerRadius { get; }
        /// <summary>Whether to show shadow on rail edge</summary>
        bool EnableRailShadow { get; }
        #endregion

        #region Interaction
        /// <summary>Whether the control is enabled</summary>
        bool IsEnabled { get; }
        /// <summary>Whether to show the collapse/expand toggle button</summary>
        bool ShowToggleButton { get; }
        /// <summary>Use SVG icons for expand/collapse instead of drawn lines</summary>
        bool UseExpandCollapseIcon { get; }
        /// <summary>SVG path for expand icon (collapsed state)</summary>
        string ExpandIconPath { get; }
        /// <summary>SVG path for collapse icon (expanded state)</summary>
        string CollapseIconPath { get; }
        /// <summary>Default icon path for items without ImagePath</summary>
        string DefaultImagePath { get; }
        /// <summary>Current control style for style-specific behavior</summary>
        TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle ControlStyle { get; }
        #endregion

        #region Animation State (NEW)
        /// <summary>Hover animation progress (0.0 to 1.0)</summary>
        float HoverAnimationProgress { get; }
        /// <summary>Selection animation progress (0.0 to 1.0)</summary>
        float SelectionAnimationProgress { get; }
        /// <summary>Per-item accordion expansion animation progress (0.0 to 1.0)</summary>
        Dictionary<SimpleItem, float> AccordionAnimationProgress { get; }
        #endregion

        #region Badges and Headers (NEW)
        /// <summary>Badge text for each item (e.g., notification count)</summary>
        Dictionary<SimpleItem, string> ItemBadges { get; }
        /// <summary>Badge colors for each item</summary>
        Dictionary<SimpleItem, Color> ItemBadgeColors { get; }
        /// <summary>Section headers to display (index, header text)</summary>
        List<(int BeforeIndex, string HeaderText)> SectionHeaders { get; }
        /// <summary>Divider positions (after which item index)</summary>
        List<int> DividerPositions { get; }
        #endregion
    }
}
