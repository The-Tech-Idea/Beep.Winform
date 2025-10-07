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
        void PaintCheckbox(Graphics g, Rectangle checkRect, bool isChecked, bool isHovered);

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
    }
}
