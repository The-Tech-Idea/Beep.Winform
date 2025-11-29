using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Figma card shadow painter - Figma design system shadows
    /// Clean, modern shadows for design tool aesthetics
    /// State-aware for interactive design components
    /// </summary>
    public static class FigmaCardShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Figma neutral shadow
            Color shadowColor = Color.Black;
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // Figma state-based shadows
            int alpha = state switch
            {
                ControlState.Hovered => 50,    // More prominent
                ControlState.Pressed => 25,    // Subtle when pressed
                ControlState.Focused => 45,    // Moderate focus
                ControlState.Selected => 55,   // Selection indication
                ControlState.Disabled => 15,   // Minimal
                _ => 35                        // Default card shadow
            };

            int spread = 2;

            // Use clean drop shadow (Figma modern style)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, alpha,
                spread);
        }
    }
}
