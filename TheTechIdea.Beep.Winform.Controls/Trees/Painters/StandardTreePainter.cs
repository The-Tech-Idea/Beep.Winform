using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Standard/classic Windows tree view painter.
    /// </summary>
    public class StandardTreePainter : BaseTreePainter
    {
        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            // Background is painted by base control
            // Individual nodes are painted via PaintNode
        }

        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            // Paint background first
            PaintNodeBackground(g, nodeBounds, isHovered, isSelected);

            // Paint toggle if has children
            if (node.Item.Children != null && node.Item.Children.Count > 0)
            {
                PaintToggle(g, node.ToggleRectContent, node.Item.IsExpanded, true, isHovered);
            }

            // Paint checkbox if enabled
            if (_owner.ShowCheckBox)
            {
                PaintCheckbox(g, node.CheckRectContent, node.Item.IsChecked, isHovered);
            }

            // Paint icon if exists
            if (!string.IsNullOrEmpty(node.Item.ImagePath))
            {
                PaintIcon(g, node.IconRectContent, node.Item.ImagePath);
            }

            // Paint text
            PaintText(g, node.TextRectContent, node.Item.Text, _owner.TextFont, isSelected, isHovered);
        }
    }
}
