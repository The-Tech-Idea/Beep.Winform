using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Painters
{
    /// <summary>
    /// Interface for tooltip painters that render different tooltip styles
    /// </summary>
    public interface IToolTipPainter
    {
        /// <summary>
        /// Paint the complete tooltip including background, border, content, and arrow
        /// </summary>
        void Paint(Graphics g, Rectangle bounds, ToolTipConfig config, ToolTipPlacement placement, IBeepTheme theme);

        /// <summary>
        /// Calculate the required size for this tooltip style
        /// </summary>
        Size CalculateSize(Graphics g, ToolTipConfig config);

        /// <summary>
        /// Paint just the background of the tooltip
        /// </summary>
        void PaintBackground(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint just the border of the tooltip
        /// </summary>
        void PaintBorder(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint just the arrow/pointer
        /// </summary>
        void PaintArrow(Graphics g, Point arrowPosition, ToolTipPlacement placement, ToolTipConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint the tooltip content (text, icon, etc.)
        /// </summary>
        void PaintContent(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint the shadow effect
        /// </summary>
        void PaintShadow(Graphics g, Rectangle bounds, ToolTipConfig config);
    }
}
