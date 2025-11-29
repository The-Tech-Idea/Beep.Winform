using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Border painter for Fluent Design (Fluent2, Windows11Mica)
    /// Includes accent bar support
    /// Fluent UX: Accent bars + rings for layered interaction feedback
    /// </summary>
    public static class FluentBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = GetColor(style, StyleColors.GetBorder, "Border", theme, useThemeColors);
            Color primaryColor = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);
            if (borderWidth <= 0f) return path;
            bool showAccentBar = false;
            int accentBarWidth = StyleBorders.GetAccentBarWidth(style);

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 60);
                    showAccentBar = true;
                    break;
                case ControlState.Pressed:
                    borderColor = primaryColor;
                    showAccentBar = true;
                    accentBarWidth = (int)(accentBarWidth * 1.3f);
                    break;
                case ControlState.Selected:
                    borderColor = primaryColor;
                    showAccentBar = true;
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 70);
                    showAccentBar = false;
                    break;
            }

            if (isFocused)
            {
                showAccentBar = true;
            }

            if (!StyleBorders.IsFilled(style))
            {
                var pen = PaintersFactory.GetPen(borderColor, borderWidth);
                
                    g.DrawPath(pen, path);
                
            }

            if (showAccentBar)
            {
                var bounds = path.GetBounds();
                BorderPainterHelpers.PaintAccentBar(g, Rectangle.Round(bounds), primaryColor, accentBarWidth);
            }

            if (isFocused)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 70);
                BorderPainterHelpers.PaintRing(g, path, focusRing, StyleBorders.GetRingWidth(style), StyleBorders.GetRingOffset(style));
            }

            // Return the area inside the border using shape-aware inset by half width
            return path.CreateInsetPath(borderWidth / 2f);
        }
        
        private static Color GetColor(BeepControlStyle style, System.Func<BeepControlStyle, Color> styleColorFunc, string themeColorKey, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return styleColorFunc(style);
        }
    }
}
