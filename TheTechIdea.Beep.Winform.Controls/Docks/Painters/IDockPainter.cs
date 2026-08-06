using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Interface for dock painters. Each DockStyle has a corresponding painter implementation.
    /// </summary>
    /// <remarks>
    /// The geometry members are what let a style be more than a recolour. An Apple dock magnifies the
    /// hovered item and displaces its neighbours; a Windows 11 dock centres fixed-size icons; a Plasma
    /// panel is a full-width bar. Those are layout differences, and until they were wired the control
    /// laid every style out identically and only painted them differently.
    ///
    /// Their signatures used to describe a different shape than the helpers that did the work:
    /// <c>CalculateItemBounds</c> took an index and returned one rectangle while
    /// <c>DockLayoutHelper</c> computed the whole set at once (making the per-index form O(n^2) per
    /// layout pass), and <c>HitTest</c> returned a <c>DockItemState</c> where all four call sites
    /// wanted the index. Rather than bridge the two, the interface now matches what the work actually
    /// is - the whole set, and an index.
    /// </remarks>
    public interface IDockPainter
    {
        /// <summary>
        /// Paint the dock background
        /// </summary>
        void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint a single dock item
        /// </summary>
        void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint the selection/hover indicator
        /// </summary>
        void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint separator between items (if enabled)
        /// </summary>
        void PaintSeparator(Graphics g, Point position, DockConfig config, IBeepTheme theme);

        /// <summary>
        /// Lays out every item at once. Override to give a style its own geometry.
        /// </summary>
        /// <param name="hoverIndex">Index of the hovered item, or -1.</param>
        /// <param name="hoverProgress">0..1 through the hover animation.</param>
        Rectangle[] CalculateItemBounds(
            Rectangle dockBounds,
            IList<SimpleItem> items,
            DockConfig config,
            int hoverIndex,
            float hoverProgress);

        /// <summary>
        /// The size the dock wants for a given item count.
        /// </summary>
        Size CalculateDockSize(int itemCount, DockConfig config);

        /// <summary>
        /// Index of the item under <paramref name="location"/>, or -1.
        /// </summary>
        int HitTest(Point location, List<DockItemState> itemStates);
    }
}
