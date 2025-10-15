using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// StripeDashboard border painter - Subtle 1px border
    /// Stripe UX: Prominent state changes with rings + accent bars
    /// </summary>
    public static class StripeDashboardBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(225, 225, 225));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(99, 91, 255));
            Color accentColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(99, 91, 255));
            Color borderColor = baseBorderColor;
            bool showRing = false;
            bool showAccentBar = false;

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.2f);
                    showRing = true;
                    break;
                case ControlState.Pressed:
                    borderColor = primaryColor;
                    showRing = true;
                    showAccentBar = true;
                    break;
                case ControlState.Selected:
                    borderColor = primaryColor;
                    showRing = true;
                    showAccentBar = true;
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 50);
                    break;
            }

            if (isFocused)
            {
                showRing = true;
                showAccentBar = true;
            }

            float borderWidth = StyleBorders.GetBorderWidth(style);
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (showRing)
            {
                Color translucentRing = BorderPainterHelpers.WithAlpha(primaryColor, 30);
                BorderPainterHelpers.PaintRing(g, path, translucentRing, 2.0f, 1.0f);
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
                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(borderWidth);
        }
    }
}

