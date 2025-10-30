using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Cyberpunk border painter - multi-layer neon glow with solid core.
    /// </summary>
    public static class CyberpunkBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color neon = useThemeColors && theme != null ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Neon);
            Color core = BorderPainterHelpers.WithAlpha(neon, 220);
            float borderWidth = StyleBorders.GetBorderWidth(style);

            // Outer glow layers
            BorderPainterHelpers.PaintGlowBorder(g, path, BorderPainterHelpers.WithAlpha(neon, 80), 12f);
            BorderPainterHelpers.PaintGlowBorder(g, path, BorderPainterHelpers.WithAlpha(neon, 120), 8f);

            if (isFocused || state == ControlState.Selected)
            {
                BorderPainterHelpers.PaintGlowBorder(g, path, BorderPainterHelpers.WithAlpha(neon, 180), 5f, 1.1f);
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, core, borderWidth, state);
            return path.CreateInsetPath(borderWidth + 4f);
        }
    }
}
