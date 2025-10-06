using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Painters
{
    /// <summary>
    /// Interface for tooltip painters
    /// Defines contract for rendering tooltips with BeepStyling integration
    /// </summary>
    public interface IToolTipPainter
    {
        /// <summary>
        /// Paint the complete tooltip including background, border, shadow, arrow, and content
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Tooltip bounds</param>
        /// <param name="config">Tooltip configuration</param>
        /// <param name="placement">Actual placement (after auto-calculation)</param>
        /// <param name="theme">Optional theme for color overrides</param>
        void Paint(Graphics g, Rectangle bounds, ToolTipConfig config, ToolTipPlacement placement, IBeepTheme theme);

        /// <summary>
        /// Paint background using BeepStyling BackgroundPainters
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Background bounds</param>
        /// <param name="config">Tooltip configuration</param>
        /// <param name="theme">Optional theme</param>
        void PaintBackground(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint border using BeepStyling BorderPainters
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Border bounds</param>
        /// <param name="config">Tooltip configuration</param>
        /// <param name="theme">Optional theme</param>
        void PaintBorder(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint shadow using BeepStyling ShadowPainters
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Shadow bounds</param>
        /// <param name="config">Tooltip configuration</param>
        void PaintShadow(Graphics g, Rectangle bounds, ToolTipConfig config);

        /// <summary>
        /// Paint arrow/pointer towards target element
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="position">Arrow tip position</param>
        /// <param name="placement">Arrow placement direction</param>
        /// <param name="config">Tooltip configuration</param>
        /// <param name="theme">Optional theme</param>
        void PaintArrow(Graphics g, Point position, ToolTipPlacement placement, ToolTipConfig config, IBeepTheme theme);

        /// <summary>
        /// Paint content (text, title, icons, images)
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Content bounds</param>
        /// <param name="config">Tooltip configuration</param>
        /// <param name="theme">Optional theme</param>
        void PaintContent(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme);

        /// <summary>
        /// Calculate optimal size for tooltip based on content
        /// </summary>
        /// <param name="g">Graphics context for measurement</param>
        /// <param name="config">Tooltip configuration</param>
        /// <returns>Calculated size</returns>
        Size CalculateSize(Graphics g, ToolTipConfig config);
    }
}
