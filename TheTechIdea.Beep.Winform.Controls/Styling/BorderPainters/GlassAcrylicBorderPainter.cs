using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// GlassAcrylic border painter - Frosted glass with subtle translucent border
    /// Glass Acrylic UX: Translucent glow opacity varies by state
    /// </summary>
    public static class GlassAcrylicBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color borderColor = BorderPainterHelpers.WithAlpha(255, 255, 255, 60);
            float borderWidth = StyleBorders.GetBorderWidth(style);
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            bool showGlow = false;
            float glowIntensity = 0.7f;

            switch (state)
            {
                case ControlState.Hovered:
                    showGlow = true;
                    glowIntensity = 0.7f;
                    break;
                case ControlState.Pressed:
                    showGlow = true;
                    glowIntensity = 0.9f;
                    break;
                case ControlState.Selected:
                    showGlow = true;
                    glowIntensity = 1.0f;
                    break;
                case ControlState.Disabled:
                    showGlow = true;
                    glowIntensity = 0.15f;
                    break;
            }

            if (isFocused)
            {
                showGlow = true;
                glowIntensity = 0.7f;
            }

            if (showGlow)
            {
                Color glowColor = BorderPainterHelpers.WithAlpha(255, 255, 255, 40);
                float glowWidth = StyleBorders.GetGlowWidth(style);
                BorderPainterHelpers.PaintGlowBorder(g, path, glowColor, glowWidth, glowIntensity);
            }

            // Return the area inside the border
                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(borderWidth);
        }
    }
}

