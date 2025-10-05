using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;


namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// Context interface that provides NavBar properties to painters
    /// </summary>
    public interface INavBarPainterContext
    {
        BindingList<SimpleItem> Items { get; }
        SimpleItem SelectedItem { get; }
        int HoveredItemIndex { get; }
        Color AccentColor { get; }
        bool UseThemeColors { get; }
        bool EnableShadow { get; }
        int CornerRadius { get; }
        int ItemWidth { get; }
        int ItemHeight { get; }
        NavBarOrientation Orientation { get; }
        IBeepTheme Theme { get; }
        void SelectItemByIndex(int index);
    }

    /// <summary>
    /// Strategy interface for painting NavBar in different visual styles.
    /// Implementations should only draw; no state should be mutated.
    /// Uses the shared BeepControlStyle enum for consistency across all navigation controls.
    /// </summary>
    public interface INavBarPainter
    {
        /// <summary>
        /// Draw the complete NavBar surface for the given bounds.
        /// </summary>
        void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds);

        /// <summary>
        /// Draw selection indicator for an item
        /// </summary>
        void DrawSelection(Graphics g, INavBarPainterContext context, Rectangle selectedRect);

        /// <summary>
        /// Draw hover effect for an item
        /// </summary>
        void DrawHover(Graphics g, INavBarPainterContext context, Rectangle hoverRect);

        /// <summary>
        /// Register hit areas for interactive elements (items, buttons, etc.)
        /// </summary>
        void UpdateHitAreas(INavBarPainterContext context, Rectangle bounds, Action<string, Rectangle, Action> registerHitArea);

        /// <summary>
        /// Friendly name for diagnostics and designer drop-downs.
        /// </summary>
        string Name { get; }
    }
}
