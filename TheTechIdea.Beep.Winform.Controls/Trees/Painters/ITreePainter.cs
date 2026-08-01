using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Interface for tree painters that render BeepTree nodes in different visual styles.
    /// </summary>
    public interface ITreePainter
    {
        /// <summary>
        /// Initialize the painter with owner and theme.
        /// </summary>
        void Initialize(BeepTree owner, IBeepTheme theme);

        /// <summary>
        /// Paint the entire tree control.
        /// </summary>
        void Paint(Graphics g, BeepTree owner, Rectangle bounds);

        /// <summary>
        /// Paint a single node.
        /// </summary>
        void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected);

        /// <summary>
        /// Paint the expand/collapse toggle button.
        /// </summary>
        void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered);

        /// <summary>
        /// Paint the checkbox for a node.
        /// </summary>
        void PaintCheckbox(Graphics g, Rectangle checkRect, bool isChecked, bool isIndeterminate, bool isHovered);

        /// <summary>
        /// Paint the icon for a node.
        /// </summary>
        void PaintIcon(Graphics g, Rectangle iconRect, string imagePath);

        /// <summary>
        /// Paint the text label for a node.
        /// </summary>
        void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered);

        /// <summary>
        /// Paint the background for a node (selection, hover effects).
        /// </summary>
        void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected);

        /// <summary>
        /// Calculate the preferred row height for a node.
        /// </summary>
        int GetPreferredRowHeight(SimpleItem item, Font font);

        /// <summary>
        /// The font this painter draws node labels with.
        /// <para>
        /// The layout measures each node's text rectangle with this font, so a painter that renders
        /// labels in something other than the tree's font — a monospace or compact variant, say —
        /// must report it here. Otherwise the rectangle is sized for one font and filled with
        /// another, and labels are clipped mid-word.
        /// </para>
        /// <para>
        /// Painters that use the tree's own font need not override the default.
        /// </para>
        /// </summary>
        Font GetNodeFont(BeepTree owner);

        /// <summary>
        /// Extra width, in pixels, this painter needs at the trailing edge of a node's text
        /// rectangle for decoration it draws there — a metric badge or status pill, for instance.
        /// <para>
        /// The layout adds this to the measured text width. A painter that instead shrinks the
        /// rectangle it is handed will squeeze the label, because the layout sized that rectangle
        /// for the text alone.
        /// </para>
        /// </summary>
        int GetLabelTrailingReserve();

        /// <summary>
        /// Paint the column headers row.
        /// </summary>
        void PaintColumnHeaders(Graphics g, Rectangle headersBounds, BeepTreeColumnCollection columns);

        /// <summary>
        /// Paint a single cell in multi-column mode.
        /// </summary>
        void PaintCell(Graphics g, Rectangle cellRect, string text, Font font, BeepTreeColumn column, bool isSelected, bool isHovered);

        /// <summary>
        /// Paint grid lines between columns and rows.
        /// </summary>
        void PaintGridLines(Graphics g, Rectangle bounds, BeepTreeColumnCollection columns, int rowCount, int rowHeight);
    }
}
