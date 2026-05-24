using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// Strategy interface for painting docking UI elements (tabs, panel chrome, splitters, etc.).
    /// 
    /// Painters are responsible for:
    /// - Drawing tab strips with active/inactive/hover states
    /// - Drawing panel title bars and borders
    /// - Drawing splitter handles
    /// - Managing theme-aware colors and fonts
    /// - Calculating preferred sizes and layouts
    /// </summary>
    public interface IDockingPainter : IDisposable
    {
        /// <summary>
        /// Gets the theme-aware color for the background surface.
        /// </summary>
        Color BackgroundColor { get; }

        /// <summary>
        /// Gets the theme-aware color for text/foreground.
        /// </summary>
        Color ForegroundColor { get; }

        /// <summary>
        /// Gets the theme-aware color for borders and separators.
        /// </summary>
        Color BorderColor { get; }

        /// <summary>
        /// Gets the theme-aware color for hovered items.
        /// </summary>
        Color HoverColor { get; }

        /// <summary>
        /// Gets the theme-aware color for selected/active items.
        /// </summary>
        Color SelectedColor { get; }

        /// <summary>
        /// Gets the theme-aware color for disabled items.
        /// </summary>
        Color DisabledColor { get; }

        /// <summary>
        /// Gets the default font for UI text.
        /// </summary>
        Font UIFont { get; }

        /// <summary>
        /// Gets the font for tab text.
        /// </summary>
        Font TabFont { get; }

        /// <summary>
        /// Gets the preferred height of a tab strip (includes padding).
        /// </summary>
        int TabStripHeight { get; }

        /// <summary>
        /// Gets the width of a splitter handle.
        /// </summary>
        int SplitterWidth { get; }

        /// <summary>
        /// Draws a tab strip with multiple tabs.
        /// </summary>
        /// <param name="graphics">Graphics context for drawing.</param>
        /// <param name="bounds">Rectangle for the entire tab strip.</param>
        /// <param name="tabs">Array of tab information.</param>
        /// <param name="activeTabIndex">Index of the active tab (-1 if none).</param>
        /// <param name="onTabClicked">Callback for tab click handling (returns tab index).</param>
        void DrawTabStrip(Graphics graphics, Rectangle bounds, TabInfo[] tabs, int activeTabIndex, Action<int> onTabClicked);

        /// <summary>
        /// Draws a single tab.
        /// </summary>
        /// <param name="graphics">Graphics context for drawing.</param>
        /// <param name="bounds">Rectangle for the tab.</param>
        /// <param name="tab">Tab information.</param>
        /// <param name="isActive">Whether this tab is currently active.</param>
        /// <param name="isHovered">Whether the tab is hovered.</param>
        void DrawTab(Graphics graphics, Rectangle bounds, TabInfo tab, bool isActive, bool isHovered);

        /// <summary>
        /// Draws a panel's title bar and borders.
        /// </summary>
        /// <param name="graphics">Graphics context for drawing.</param>
        /// <param name="bounds">Rectangle for the panel.</param>
        /// <param name="title">Panel title text.</param>
        /// <param name="icon">Optional icon image.</param>
        /// <param name="isDirty">Whether the panel has unsaved changes.</param>
        void DrawPanelChrome(Graphics graphics, Rectangle bounds, string title, Image icon, bool isDirty);

        /// <summary>
        /// Draws a splitter handle.
        /// </summary>
        /// <param name="graphics">Graphics context for drawing.</param>
        /// <param name="bounds">Rectangle for the splitter.</param>
        /// <param name="orientation">Is the splitter vertical or horizontal?</param>
        /// <param name="isHovered">Whether the splitter is hovered.</param>
        void DrawSplitter(Graphics graphics, Rectangle bounds, SplitterOrientation orientation, bool isHovered);

        /// <summary>
        /// Draws a docking guide overlay (the visual guide when dragging a panel).
        /// </summary>
        /// <param name="graphics">Graphics context for drawing.</param>
        /// <param name="bounds">Target rectangle where the panel would dock.</param>
        /// <param name="position">Which edge (Left, Right, Top, Bottom, Fill) is being targeted.</param>
        void DrawDockingGuide(Graphics graphics, Rectangle bounds, DockPosition position);

        /// <summary>
        /// Calculates the preferred size for a tab strip given the available width.
        /// </summary>
        /// <param name="tabs">Array of tabs.</param>
        /// <param name="availableWidth">Available horizontal space.</param>
        /// <returns>Preferred size (width, height).</returns>
        Size GetTabStripPreferredSize(TabInfo[] tabs, int availableWidth);

        /// <summary>
        /// Calculates the tab index at the given point, or -1 if no tab is at that point.
        /// </summary>
        /// <param name="point">Point in tab strip coordinates.</param>
        /// <param name="bounds">Bounds of the tab strip.</param>
        /// <param name="tabs">Array of tabs.</param>
        /// <returns>Tab index or -1.</returns>
        int GetTabAtPoint(Point point, Rectangle bounds, TabInfo[] tabs);

        /// <summary>
        /// Calculates the close button rectangle for a given tab.
        /// Returns Rectangle.Empty if the tab has no close button.
        /// </summary>
        /// <param name="tabBounds">Bounds of the tab.</param>
        /// <param name="tab">Tab information.</param>
        /// <returns>Close button rectangle or empty.</returns>
        Rectangle GetTabCloseButtonRect(Rectangle tabBounds, TabInfo tab);

        /// <summary>
        /// Invalidates any cached resources (e.g., when theme changes).
        /// Called when the active theme switches.
        /// </summary>
        void InvalidateCache();
    }

    /// <summary>
    /// Information about a single tab in the tab strip.
    /// </summary>
    public struct TabInfo
    {
        /// <summary>Unique identifier for this tab.</summary>
        public string Key { get; set; }

        /// <summary>Display title text.</summary>
        public string Title { get; set; }

        /// <summary>Optional icon image.</summary>
        public Image Icon { get; set; }

        /// <summary>Whether this tab has unsaved changes (display indicator).</summary>
        public bool IsDirty { get; set; }

        /// <summary>Whether this tab can be closed.</summary>
        public bool CanClose { get; set; }

        /// <summary>Optional user data associated with this tab.</summary>
        public object Tag { get; set; }

        public override string ToString() => $"Tab[{Key}, {Title}]";
    }

    /// <summary>
    /// Orientation of a splitter handle.
    /// </summary>
    public enum SplitterOrientation
    {
        /// <summary>Horizontal splitter (mouse Y changes).</summary>
        Horizontal = 0,

        /// <summary>Vertical splitter (mouse X changes).</summary>
        Vertical = 1
    }
}
