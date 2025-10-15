using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Fluent2 border painter - accent bar on left when focused + border (sizes from StyleBorders)
    /// Fluent2 UX: Accent bars indicate interaction intensity
    /// </summary>
    public static class Fluent2BorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(96, 94, 92));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(0, 120, 212));
            Color accentColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(0, 120, 212));
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);
            int accentBarWidth = StyleBorders.GetAccentBarWidth(style);
            bool showAccentBar = false;
            int accentAlpha = 180;

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.3f);
                    showAccentBar = true;
                    accentAlpha = 180;
                    break;
                case ControlState.Pressed:
                    borderColor = BorderPainterHelpers.Darken(primaryColor, 0.15f);
                    showAccentBar = true;
                    accentBarWidth = (int)(accentBarWidth * 1.5f);
                    accentAlpha = 255;
                    break;
                case ControlState.Selected:
                    borderColor = primaryColor;
                    showAccentBar = true;
                    accentAlpha = 255;
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 80);
                    showAccentBar = false;
                    break;
            }

            if (isFocused)
            {
                borderColor = primaryColor;
                showAccentBar = true;
                accentAlpha = 255;
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (showAccentBar)
            {
                var bounds = path.GetBounds();
                Color finalAccentColor = BorderPainterHelpers.WithAlpha(accentColor, accentAlpha);
                BorderPainterHelpers.PaintAccentBar(g, Rectangle.Round(bounds), finalAccentColor, accentBarWidth);
            }

            // Return the area inside the border
                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(borderWidth);
        }
    }
}

