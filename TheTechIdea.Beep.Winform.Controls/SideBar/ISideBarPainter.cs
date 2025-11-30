using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    /// <summary>
    /// Interface for BeepSideBar painters - defines how different visual styles are rendered.
    /// Each painter should be DISTINCT with unique visual identity for:
    /// - Selection indicator style (left bar, pill, glow, underline, etc.)
    /// - Hover effects (tint, reveal, scale, glow, etc.)
    /// - Accordion expansion style
    /// - Icon rendering approach
    /// </summary>
    public interface ISideBarPainter : IDisposable
    {
        /// <summary>
        /// Name of the painter style
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
        /// Draws the pressed/active state for a menu item
        /// </summary>
        void PaintPressed(Graphics g, Rectangle itemRect, ISideBarPainterContext context);

        /// <summary>
        /// Draws the disabled state for a menu item
        /// </summary>
        void PaintDisabled(Graphics g, Rectangle itemRect, ISideBarPainterContext context);

        /// <summary>
        /// Draws the expand/collapse toggle button
        /// </summary>
        void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context);

        /// <summary>
        /// Draws child items (accordion children)
        /// </summary>
        void PaintChildItem(Graphics g, SimpleItem childItem, Rectangle childRect, ISideBarPainterContext context, int indentLevel);

        /// <summary>
        /// Draws the expand/collapse icon for accordion items.
        /// Each painter should have a distinct style (chevron, plus/minus, triangle, etc.)
        /// </summary>
        void PaintExpandIcon(Graphics g, Rectangle iconRect, bool isExpanded, SimpleItem item, ISideBarPainterContext context);

        /// <summary>
        /// Draws connector lines between parent and child items in accordion.
        /// Each painter should have a distinct style (dotted, solid, tree-view, none, etc.)
        /// </summary>
        void PaintAccordionConnector(Graphics g, SimpleItem parent, Rectangle parentRect, SimpleItem child, Rectangle childRect, int indentLevel, ISideBarPainterContext context);

        /// <summary>
        /// Draws a badge/notification indicator on a menu item
        /// </summary>
        void PaintBadge(Graphics g, Rectangle itemRect, string badgeText, Color badgeColor, ISideBarPainterContext context);

        /// <summary>
        /// Draws a section header/divider with text
        /// </summary>
        void PaintSectionHeader(Graphics g, Rectangle headerRect, string headerText, ISideBarPainterContext context);

        /// <summary>
        /// Draws a simple divider line
        /// </summary>
        void PaintDivider(Graphics g, Rectangle dividerRect, ISideBarPainterContext context);
    }
}
