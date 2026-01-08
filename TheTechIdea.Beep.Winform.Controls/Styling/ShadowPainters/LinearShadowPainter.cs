using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Linear style shadow painter - Minimal shadows, almost flat
    /// </summary>
    public static class LinearShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            
            // Linear style: very minimal shadows, almost flat design
            // Only show shadow on hover/focus for subtle depth
            if (state == ControlState.Hovered || state == ControlState.Focused)
            {
                return ShadowPainterHelpers.PaintSubtleShadow(g, path, radius, offsetY: 1, alpha: 8);
            }

            return path; // No shadow for normal state
        }
    }
}
