using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painter for GlassAcrylic Style
    /// </summary>
    public static class GlassBackgroundPainter
    {
        /// <summary>
        /// Paint glass/acrylic background
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            // Base mica/glass color (matches FormStyle GlassFormPainter)
            Color glassColor = Color.FromArgb(220, 245, 245, 245); // Semi-transparent light gray
            var glassBrush = PaintersFactory.GetSolidBrush(glassColor);
            g.FillPath(glassBrush, path);
        }
    }
}

