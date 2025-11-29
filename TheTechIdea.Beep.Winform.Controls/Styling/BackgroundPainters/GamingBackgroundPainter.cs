using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Gaming background painter - RGB gaming aesthetic
    /// Dark background with angular edges (no anti-aliasing)
    /// Green neon glow on hover for aggressive feedback
    /// </summary>
    public static class GamingBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Gaming: dark charcoal
            Color darkBg = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : StyleColors.GetBackground(BeepControlStyle.Gaming);
            Color neonGreen = useThemeColors && theme != null 
                ? theme.AccentColor 
                : StyleColors.GetPrimary(BeepControlStyle.Gaming);

            // Angular gaming aesthetic - disable smoothing
            using (var scope = new BackgroundPainterHelpers.SmoothingScope(g, SmoothingMode.None))
            {
                // Strong state feedback for gaming
                BackgroundPainterHelpers.PaintSolidBackground(g, path, darkBg, state,
                    BackgroundPainterHelpers.StateIntensity.Strong);
            }

            // Neon glow overlay on hover
            if (state == ControlState.Hovered)
            {
                var glowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(25, neonGreen));
                g.FillPath(glowBrush, path);
            }
        }
    }
}
