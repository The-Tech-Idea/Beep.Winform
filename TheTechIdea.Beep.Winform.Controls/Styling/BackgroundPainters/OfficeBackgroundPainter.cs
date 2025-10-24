using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Office background painter - Microsoft Office Ribbon UI Style
    /// Clean white background with subtle gradients and professional appearance
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class OfficeBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Office: Clean white background with subtle gradients (ribbon Style)
            Color backgroundColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Office);
            Color primaryColor = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Office);

            // Office-specific state handling - Professional, subtle
            Color topColor, bottomColor;
            
            switch (state)
            {
                case ControlState.Hovered:
                    // Office hover: Very light blue tint with subtle gradient
                    topColor = Color.FromArgb(250, 251, 253); // Very light blue
                    bottomColor = Color.FromArgb(245, 248, 252); // Slightly darker blue
                    break;
                case ControlState.Pressed:
                    // Office pressed: Light primary color with gradient
                    topColor = Color.FromArgb(210, 230, 250); // Light blue
                    bottomColor = Color.FromArgb(190, 220, 245); // Slightly darker
                    break;
                case ControlState.Selected:
                    // Office selected: Accent color with subtle gradient
                    int alpha = 40;
                    topColor = Color.FromArgb(alpha, primaryColor.R, primaryColor.G, primaryColor.B);
                    bottomColor = Color.FromArgb(alpha + 20, primaryColor.R, primaryColor.G, primaryColor.B);
                    
                    // Fill base white first
                    using (var baseBrush = new SolidBrush(backgroundColor))
                    {
                        g.FillPath(baseBrush, path);
                    }
                    break;
                case ControlState.Focused:
                    // Office focused: Very subtle blue tint
                    topColor = Color.FromArgb(252, 253, 254);
                    bottomColor = Color.FromArgb(248, 250, 252);
                    break;
                case ControlState.Disabled:
                    // Office disabled: Gray with subtle gradient
                    topColor = Color.FromArgb(240, 240, 240);
                    bottomColor = Color.FromArgb(230, 230, 230);
                    break;
                default: // Normal
                    // Office normal: Pure white (clean professional)
                    topColor = backgroundColor;
                    bottomColor = backgroundColor;
                    break;
            }

            // Paint with subtle vertical gradient (Office ribbon Style)
            var bounds = path.GetBounds();
            using (var brush = new LinearGradientBrush(
                new PointF(bounds.Left, bounds.Top),
                new PointF(bounds.Left, bounds.Bottom),
                topColor, bottomColor))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
