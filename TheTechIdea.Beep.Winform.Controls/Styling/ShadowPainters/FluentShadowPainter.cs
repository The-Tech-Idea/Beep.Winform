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
            float opacity = state switch
            {
                ControlState.Hovered => 0.4f,    // More prominent on hover (reveal effect)
                ControlState.Pressed => 0.2f,    // Reduced when pressed
                ControlState.Focused => 0.35f,    // Moderate focus
                ControlState.Selected => 0.45f,   // More for selection indication
                ControlState.Disabled => 0.1f,   // Minimal
                _ => 0.3f                        // Default subtle shadow
            };

            // Use soft shadow with more layers for Fluent depth
            return ShadowPainterHelpers.PaintSoftShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, opacity,
                6);
        }
    }
}
