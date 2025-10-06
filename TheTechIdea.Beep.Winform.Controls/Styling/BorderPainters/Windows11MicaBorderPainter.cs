using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Windows11Mica border painter - Subtle 1px border with mica effect
    /// Windows 11 UX: Very subtle state changes, Mica material design
    /// </summary>
    public static class Windows11MicaBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get base colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(96, 94, 92));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(0, 120, 212));
            Color accentColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(0, 120, 212));
            
            Color borderColor = baseBorderColor;
            bool showAccentBar = false;

            // Windows 11 Mica UX: Very subtle state changes
            switch (state)
            {
                case ControlState.Hovered:
                    // Windows 11: Subtle border color shift (blend with primary)
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.2f);
                    break;
                    
                case ControlState.Pressed:
                    // Windows 11: Slightly darker border
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.4f);
                    break;
                    
                case ControlState.Selected:
                    // Windows 11: Accent color border
                    borderColor = accentColor;
                    showAccentBar = true;
                    break;
                    
                case ControlState.Disabled:
                    // Windows 11: Very subtle disabled (20% opacity)
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 50);
                    break;
            }

            // Focus overrides state
            if (isFocused)
            {
                showAccentBar = true;
            }

            // Paint border
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, StyleBorders.GetBorderWidth(style), state);

            // Windows 11: Add focus glow and accent bar
            if (isFocused)
            {
                // Subtle focus glow
                Color subtleGlow = BorderPainterHelpers.WithAlpha(primaryColor, 30); // Very subtle Windows glow
                BorderPainterHelpers.PaintGlowBorder(g, path, subtleGlow, 1.0f, 0.8f);
            }
            
            // Accent bar for Windows 11 style
            if (showAccentBar)
            {
                var bounds = path.GetBounds();
                int accentBarWidth = StyleBorders.GetAccentBarWidth(style); // 3px for Windows11Mica
                if (accentBarWidth > 0)
                {
                    BorderPainterHelpers.PaintAccentBar(g, Rectangle.Round(bounds), accentColor, accentBarWidth);
                }
            }
        }
    }
}

