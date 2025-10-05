using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// DarkGlow border painter - Cyan glow border effect
    /// </summary>
    public static class DarkGlowBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            BorderPainterHelpers.ControlState state = BorderPainterHelpers.ControlState.Normal)
        {
            // DarkGlow uses glow effect instead of solid border
            Color glowColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(0, 255, 255));
            float glowIntensity = isFocused ? 1.2f : 0.8f;
            
            BorderPainterHelpers.PaintGlowBorder(g, path, glowColor, 2f, glowIntensity);
        }
    }
}
