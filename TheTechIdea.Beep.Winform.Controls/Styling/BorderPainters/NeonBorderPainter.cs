using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
 
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    public static class NeonBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return path;

            // Base neon color
            Color baseNeon = useThemeColors && theme != null
                ? theme.AccentColor
                : StyleColors.GetPrimary(BeepControlStyle.Neon);

            // Adjust border/glow color by state
            Color neonColor = state switch
            {
                ControlState.Hovered => BorderPainterHelpers.Lighten(baseNeon, 0.1f),
                ControlState.Pressed => BorderPainterHelpers.Darken(baseNeon, 0.1f),
                ControlState.Disabled => BorderPainterHelpers.WithAlpha(baseNeon, 90),
                _ => baseNeon
            };

            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Neon);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw sharp neon border
            BorderPainterHelpers.PaintSimpleBorder(g, path, neonColor, borderWidth, state);

            // Neon glow intensity varies with state/focus
            float glowWidth = isFocused || state == ControlState.Selected ? 8.0f : 6.0f;
            float glowIntensity = state == ControlState.Disabled ? 0.5f : 1.0f;
            BorderPainterHelpers.PaintGlowBorder(g, path, neonColor, glowWidth, glowIntensity);

            // Return area inside border so content paths are inset correctly
            return path.CreateInsetPath(borderWidth);
        }
    }
}
