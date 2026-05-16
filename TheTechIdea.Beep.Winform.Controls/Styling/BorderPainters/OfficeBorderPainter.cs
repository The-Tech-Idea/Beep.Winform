using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

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
            Color baseBorderColor = useThemeColors && theme != null ? theme.BorderColor : StyleColors.GetBorder(style);
            Color primaryColor = useThemeColors && theme != null ? theme.AccentColor : StyleColors.GetPrimary(style);
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
            float borderWidth = StyleBorders.GetBorderWidth(style); // 1.0f
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // Paint Office signature accent bar on left (ribbon style) as a thick stroke,
            // inset by half its width so the stroke stays fully inside the control boundary.
            if (showAccentBar)
            {
                int accentBarWidth = StyleBorders.GetAccentBarWidth(style); // 3px
                var bounds = path.GetBounds();
                float halfBar = accentBarWidth / 2f;

                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var accentPen = new Pen(primaryColor, accentBarWidth))
                {
                    accentPen.StartCap = LineCap.Flat;
                    accentPen.EndCap   = LineCap.Flat;
                    // Draw a vertical line inset by halfBar from the left edge
                    g.DrawLine(accentPen,
                        bounds.Left + halfBar, bounds.Top,
                        bounds.Left + halfBar, bounds.Bottom);
                }
            }

            // Return the area inside the border
            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}
