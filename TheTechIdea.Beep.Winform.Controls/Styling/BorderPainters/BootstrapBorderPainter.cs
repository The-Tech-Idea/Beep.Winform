using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Bootstrap border painter - Bootstrap primary blue 1px border
    /// Bootstrap UX: Bold, clear state changes with focus rings + accent bars
    /// </summary>
    public static class BootstrapBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get Bootstrap colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(206, 212, 218));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(13, 110, 253));
            
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);
            bool showAccentBar = false;

            // Bootstrap UX: Bold, clear state changes
            switch (state)
            {
                case ControlState.Hovered:
                    // Bootstrap: Subtle primary tint
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.2f);
                    break;
                    
                case ControlState.Pressed:
                    // Bootstrap: Bold primary border + thicker
                    borderColor = primaryColor;
                    borderWidth = Math.Max(borderWidth * 1.5f, 2.0f);
                    showAccentBar = true;
                    break;
                    
                case ControlState.Selected:
                    // Bootstrap: Primary border + accent bar
                    borderColor = primaryColor;
                    showAccentBar = true;
                    break;
                    
                case ControlState.Disabled:
                    // Bootstrap: Gray disabled
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 60);
                    break;
            }

            // Paint border
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // Bootstrap: Add focus ring and accent bar
            if (isFocused)
            {
                // Focus ring (Bootstrap blue)
                Color translucentRing = BorderPainterHelpers.WithAlpha(primaryColor, 25); // Bootstrap focus ring opacity
                BorderPainterHelpers.PaintRing(g, path, translucentRing, 2.0f, 1.0f);
                showAccentBar = true;
            }
            
            // Accent bar for Bootstrap components
            if (showAccentBar)
            {
                var bounds = path.GetBounds();
                int accentBarWidth = StyleBorders.GetAccentBarWidth(style);
                if (accentBarWidth > 0)
                {
                    BorderPainterHelpers.PaintAccentBar(g, Rectangle.Round(bounds), primaryColor, accentBarWidth);
                }
            }

                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(borderWidth);
        }
    }
}
