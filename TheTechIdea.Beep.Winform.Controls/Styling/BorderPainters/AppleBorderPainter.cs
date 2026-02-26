using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Border painter for Apple styles (iOS15, MacOSBigSur)
    /// Apple UX: Minimal, refined state changes with subtle focus rings
    /// </summary>
    public static class AppleBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Apple UX: Subtle outlined borders with focus rings
            if (StyleBorders.IsFilled(style))
                return path;

            Color baseBorderColor = GetColor(style, StyleColors.GetBorder, "Border", theme, useThemeColors);
            Color primaryColor = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
            
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);
            if (borderWidth <= 0f) return path;
            if (!isFocused && state == ControlState.Normal)
            {
                // Keep Apple subtle, but align idle clarity with other core styles.
                borderWidth = Math.Max(borderWidth, 1.15f);
                borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, Math.Max((int)baseBorderColor.A, 165));
            }

            // Apple UX: Minimal, refined state changes (very subtle)
            switch (state)
            {
                case ControlState.Hovered:
                    // Apple: Very subtle hover tint (30 alpha for minimal feedback)
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 30);
                    break;
                    
                case ControlState.Pressed:
                    // Apple: Slightly stronger tint (60 alpha, still minimal)
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 60);
                    break;
                    
                case ControlState.Selected:
                    // Apple: Subtle primary with transparency (120 alpha)
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 120);
                    break;
                    
                case ControlState.Disabled:
                    // Apple: Very light disabled (40 alpha for refined disabled state)
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 40);
                    break;
            }

            borderColor = BorderPainterHelpers.EnsureVisibleBorderColor(borderColor, theme, state);
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // Apple: Add subtle focus rings (Apple blue)
            if (isFocused)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 50); // Subtle Apple focus ring
                BorderPainterHelpers.PaintRing(g, path, focusRing, StyleBorders.GetRingWidth(style), StyleBorders.GetRingOffset(style));
            }

            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
        
        private static Color GetColor(BeepControlStyle style, System.Func<BeepControlStyle, Color> styleColorFunc, string themeColorKey, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(theme, themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return styleColorFunc(style);
        }
    }
}
