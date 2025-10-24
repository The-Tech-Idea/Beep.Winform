using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Office border painter - Microsoft Office Ribbon UI Style
    /// 1px borders with 4px radius, 3px blue accent bar on left (ribbon signature)
    /// Professional, clean appearance with subtle rounded corners
    /// </summary>
    public static class OfficeBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(BeepControlStyle.Office);
            Color primaryColor = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Office);
            Color borderColor = baseBorderColor;
            bool showAccentBar = false;

            // Office: Professional, subtle state changes
            switch (state)
            {
                case ControlState.Hovered:
                    // Office hover: Light primary color border
                    borderColor = Color.FromArgb(150, primaryColor);
                    break;
                case ControlState.Pressed:
                    // Office pressed: Full primary color border
                    borderColor = primaryColor;
                    showAccentBar = true;
                    break;
                case ControlState.Selected:
                    // Office selected: Primary color with accent bar
                    borderColor = primaryColor;
                    showAccentBar = true;
                    break;
                case ControlState.Disabled:
                    // Office disabled: Very light border
                    borderColor = Color.FromArgb(100, baseBorderColor);
                    break;
            }

            // Office focused: Show accent bar
            if (isFocused)
            {
                borderColor = primaryColor;
                showAccentBar = true;
            }

            // Paint Office 1px border with 4px radius
            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Office); // 1.0f
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // Paint Office signature 3px accent bar on left (ribbon Style)
            if (showAccentBar)
            {
                int accentBarWidth = StyleBorders.GetAccentBarWidth(BeepControlStyle.Office); // 3px
                var bounds = path.GetBounds();
                
                using (var accentBrush = new SolidBrush(primaryColor))
                {
                    // Draw vertical accent bar on left edge
                    RectangleF accentRect = new RectangleF(
                        bounds.Left, 
                        bounds.Top, 
                        accentBarWidth, 
                        bounds.Height
                    );
                    g.FillRectangle(accentBrush, accentRect);
                }
            }

            // Return the area inside the border
            return path.CreateInsetPath(borderWidth);
        }
    }
}
