using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    /// <summary>
    /// Interface for BeepSideBar painters - defines how different visual styles are rendered
    /// </summary>
    public interface ISideBarPainter
    {
        /// <summary>
        /// Name of the painter Style
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Main paint method - draws the entire sidebar
        /// </summary>
        void Paint(ISideBarPainterContext context);

        /// <summary>
        /// Draws a single menu item
        /// </summary>
        void PaintMenuItem(Graphics g, SimpleItem item, Rectangle itemRect, ISideBarPainterContext context);

        /// <summary>
        /// Draws the selection indicator for a menu item
        /// </summary>
        void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context);

        /// <summary>
        /// Draws the hover effect for a menu item
        /// </summary>
        void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context);

        /// <summary>
        /// Draws the expand/collapse toggle button
        /// </summary>
        void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context);

        /// <summary>
        /// Draws child items (accordion children)
        /// </summary>
        void PaintChildItem(Graphics g, SimpleItem childItem, Rectangle childRect, ISideBarPainterContext context, int indentLevel);
    }
}
