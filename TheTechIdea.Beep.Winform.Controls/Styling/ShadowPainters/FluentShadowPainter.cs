using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Fluent Design shadow painter - Microsoft Fluent Design System
    /// Layered depth through subtle shadows with reveal lighting feel
    /// State-aware for interactive feedback
    /// </summary>
    public static class FluentShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Fluent shadow color - neutral black
            Color shadowColor = Color.Black;
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // State-based shadow for Fluent layered depth
            int alpha = state switch
            {
                ControlState.Hovered => 50,    // More prominent on hover (reveal effect)
                ControlState.Pressed => 25,    // Reduced when pressed
                ControlState.Focused => 45,    // Moderate focus
                ControlState.Selected => 55,   // More for selection indication
                ControlState.Disabled => 10,   // Minimal
                _ => 35                        // Default subtle shadow
            };

            int spread = state == ControlState.Hovered ? 3 : 2;

            // Use clean drop shadow (Fluent layered depth)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, alpha,
                spread);
        }
    }
}
