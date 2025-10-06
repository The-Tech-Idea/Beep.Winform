using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Border painter for web framework styles (Bootstrap, Tailwind, Stripe, Figma, Discord, AntDesign, Chakra)
    /// Web Framework UX: Standardized state behaviors across frameworks
    /// </summary>
    public static class WebFrameworkBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Web Framework UX: Outlined borders with standardized ring effects
            if (StyleBorders.IsFilled(style))
                return;

            Color baseBorderColor = GetColor(style, StyleColors.GetBorder, "Border", theme, useThemeColors);
            Color primaryColor = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
            
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);
            bool showRing = false;

            // Web Framework: Standardized state behaviors
            switch (state)
            {
                case ControlState.Hovered:
                    // Web Framework: Subtle hover tint (50 alpha standard)
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 50);
                    showRing = true; // Preview ring
                    break;
                    
                case ControlState.Pressed:
                    // Web Framework: Primary border with ring
                    borderColor = primaryColor;
                    borderWidth = Math.Max(borderWidth * 1.2f, 1.5f);
                    showRing = true;
                    break;
                    
                case ControlState.Selected:
                    // Web Framework: Full primary + ring
                    borderColor = primaryColor;
                    showRing = true;
                    break;
                    
                case ControlState.Disabled:
                    // Web Framework: Standard disabled
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 50);
                    break;
            }

            // Focus overrides state
            if (isFocused)
            {
                showRing = true;
            }

            // Paint border
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // Web Framework: Standardized focus ring effects for all frameworks
            if (showRing)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 60); // Standardized opacity
                BorderPainterHelpers.PaintRing(g, path, focusRing, StyleBorders.GetRingWidth(style), StyleBorders.GetRingOffset(style));
            }
        }
        
        private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            
            int diameter = radius * 2;
            System.Drawing.Size size = new System.Drawing.Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
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
