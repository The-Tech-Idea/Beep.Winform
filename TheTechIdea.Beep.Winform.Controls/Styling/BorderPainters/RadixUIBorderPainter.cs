using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Radix UI border painter - Accessible, clear borders with enhanced focus indicators
    /// </summary>
    public static class RadixUIBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border",
                Color.FromArgb(214, 211, 209)); // Radix UI default
            
            float borderWidth = 1f;

            if (isFocused)
            {
                // Radix UI uses prominent focus rings for accessibility
                BorderPainterHelpers.PaintFocusRing(g, path,
                    BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary",
                        Color.FromArgb(0, 122, 255)), // Radix blue
                    2f, BorderPainterHelpers.FocusRingStyle.Outline);
                
                borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary",
                    Color.FromArgb(0, 122, 255));
                borderWidth = 2f;
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
            return path.CreateInsetPath(borderWidth);
        }
    }
}
