using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Border painter for web framework styles (Bootstrap, Tailwind, Stripe, Figma, Discord, AntDesign, Chakra)
    /// Web Framework UX: Standardized state behaviors across frameworks
    /// </summary>
    public static class WebFrameworkBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (StyleBorders.IsFilled(style))
                return path;

            Color baseBorderColor = GetColor(style, StyleColors.GetBorder, "Border", theme, useThemeColors);
            Color primaryColor = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);
            bool showRing = false;

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 50);
                    showRing = true;
                    break;
                case ControlState.Pressed:
                    borderColor = primaryColor;
                    borderWidth = Math.Max(borderWidth * 1.2f, 1.5f);
                    showRing = true;
                    break;
                case ControlState.Selected:
                    borderColor = primaryColor;
                    showRing = true;
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 50);
                    break;
            }

            if (isFocused)
            {
                showRing = true;
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (showRing)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 60);
                BorderPainterHelpers.PaintRing(g, path, focusRing, StyleBorders.GetRingWidth(style), StyleBorders.GetRingOffset(style));
            }

            // Return the area inside the border
                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(borderWidth);
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
