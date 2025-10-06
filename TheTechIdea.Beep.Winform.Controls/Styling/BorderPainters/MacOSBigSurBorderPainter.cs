using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// MacOSBigSur border painter - Clean 1px border with system colors
    /// macOS UX: Refined vibrancy with subtle state transitions
    /// </summary>
    public static class MacOSBigSurBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get macOS system colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(209, 209, 214));
            Color accentColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(0, 122, 255));
            
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style); // 0.5f for macOS

            // macOS Big Sur UX: Refined vibrancy with subtle state transitions
            switch (state)
            {
                case ControlState.Hovered:
                    // macOS: Very subtle vibrancy tint (10% blend)
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, accentColor, 0.10f);
                    break;
                    
                case ControlState.Pressed:
                    // macOS: Slightly more prominent vibrancy (25% blend)
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, accentColor, 0.25f);
                    break;
                    
                case ControlState.Selected:
                    // macOS: Accent color with vibrancy transparency
                    borderColor = BorderPainterHelpers.WithAlpha(accentColor, 160);
                    break;
                    
                case ControlState.Disabled:
                    // macOS: Highly transparent disabled for vibrancy
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 50);
                    break;
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // macOS: Add focus ring (system blue)
            if (isFocused)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(accentColor, 80); // macOS focus ring opacity
                BorderPainterHelpers.PaintRing(g, path, focusRing, 2.0f, 1.0f);
            }
        }
    }
}

