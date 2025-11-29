using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Windows11Mica border painter - Subtle 1px border with mica effect
    /// Windows 11 UX: Very subtle state changes, Mica material design
    /// </summary>
    public static class Windows11MicaBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(96, 94, 92));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(0, 120, 212));
            Color accentColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(0, 120, 212));
            Color borderColor = baseBorderColor;
            bool showAccentBar = false;

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.2f);
                    break;
                case ControlState.Pressed:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.4f);
                    break;
                case ControlState.Selected:
                    borderColor = accentColor;
                    showAccentBar = true;
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 50);
                    break;
            }

            if (isFocused)
            {
                showAccentBar = true;
            }

            float borderWidth = StyleBorders.GetBorderWidth(style);
            if (borderWidth <= 0f) return path;
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (isFocused)
            {
                Color subtleGlow = BorderPainterHelpers.WithAlpha(primaryColor, 30);
                BorderPainterHelpers.PaintGlowBorder(g, path, subtleGlow, 1.0f, 0.8f);
            }

            if (showAccentBar)
            {
                var bounds = path.GetBounds();
                int accentBarWidth = StyleBorders.GetAccentBarWidth(style);
                if (accentBarWidth > 0)
                {
                    BorderPainterHelpers.PaintAccentBar(g, Rectangle.Round(bounds), accentColor, accentBarWidth);
                }
            }

            // Return the area inside the border
            var boundsInner = path.GetBounds();
                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(borderWidth);
        }
    }
}

