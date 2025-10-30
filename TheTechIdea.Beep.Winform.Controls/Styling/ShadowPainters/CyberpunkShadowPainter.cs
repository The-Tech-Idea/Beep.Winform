using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Cyberpunk shadow painter - cyan neon glow.
    /// </summary>
    public static class CyberpunkShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(style))
                return path;

            Color glow = useThemeColors && theme != null ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Neon);
            return ShadowPainterHelpers.PaintNeonGlow(g, path, radius, glow, 1.0f, StyleShadows.GetShadowBlur(style));
        }
    }
}
