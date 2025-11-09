using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Interface for dock painters
    /// Each DockStyle has a corresponding painter implementation
    /// </summary>
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
        /// Calculate item bounds based on index and state
        /// </summary>
        Rectangle CalculateItemBounds(int index, List<DockItemState> allStates, DockConfig config, Rectangle dockBounds);

        /// <summary>
        /// Calculate the total dock bounds
        /// </summary>
        Rectangle CalculateDockBounds(List<DockItemState> itemStates, DockConfig config, Size containerSize);

        /// <summary>
        /// Perform hit test on point
        /// </summary>
        DockItemState HitTest(Point location, List<DockItemState> itemStates, DockConfig config);
    }
}
