using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Linear style border painter - Ultra-clean, minimal borders
    /// </summary>
    public static class LinearBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Linear style: very subtle borders, almost invisible
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border",
                Color.FromArgb(240, 240, 240)); // Very light gray
            
            float borderWidth = 0.5f; // Ultra-thin borders

            if (isFocused)
            {
                borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary",
                    Color.FromArgb(99, 102, 241)); // Linear indigo
                borderWidth = 1f;
            }

            borderColor = BorderPainterHelpers.EnsureVisibleBorderColor(borderColor, theme, state);
            if (borderWidth > 0)
            {
                BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
            }
            
            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}
