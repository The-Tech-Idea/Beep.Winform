using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// DarkGlow border painter - Cyan glow border effect
    /// DarkGlow UX: Glow intensity varies dramatically by state
    /// </summary>
    public static class DarkGlowBorderPainter 
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get DarkGlow colors
            Color glowColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(0, 255, 255));
            float glowWidth = StyleBorders.GetGlowWidth(style); // 2.0f for DarkGlow
            float glowIntensity;

            // DarkGlow UX: Dramatic glow intensity changes by state
            switch (state)
            {
                case ControlState.Hovered:
                    glowIntensity = 0.8f;
                    break;
                case ControlState.Pressed:
                    glowIntensity = 1.0f;
                    glowWidth *= 1.3f;
                    break;
                case ControlState.Selected:
                    glowIntensity = 1.2f;
                    break;
                case ControlState.Disabled:
                    glowIntensity = 0.2f;
                    break;
                default:
                    glowIntensity = 0.6f;
                    break;
            }

            if (isFocused)
            {
                glowIntensity = 1.5f;
            }

            BorderPainterHelpers.PaintGlowBorder(g, path, glowColor, glowWidth, glowIntensity);

                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(glowWidth);
        }
    }
}
