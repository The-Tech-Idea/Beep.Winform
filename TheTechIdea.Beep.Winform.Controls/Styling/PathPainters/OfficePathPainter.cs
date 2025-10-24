using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Office path painter - Microsoft Office Ribbon UI Style
    /// 4px rounded corners with subtle gradients, professional blue colors
    /// Clean, professional appearance matching Office ribbon aesthetic
    /// </summary>
    public static class OfficePathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            // Office: Professional blue with subtle gradient
            Color primaryColor = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Office);
            int officeRadius = StyleBorders.GetRadius(BeepControlStyle.Office); // 4px subtle rounded

            // Create Office rounded rectangle path
            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, officeRadius))
            {
                // Office: Subtle vertical gradient (ribbon Style)
                Color topColor = primaryColor;
                Color bottomColor = Color.FromArgb(
                    primaryColor.A,
                    Math.Max(0, primaryColor.R - 20),
                    Math.Max(0, primaryColor.G - 20),
                    Math.Max(0, primaryColor.B - 20)
                );

                using (var brush = new LinearGradientBrush(
                    new Point(bounds.Left, bounds.Top),
                    new Point(bounds.Left, bounds.Bottom),
                    topColor, bottomColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }
    }
}
