using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// HighContrast border painter - WCAG AAA accessibility compliance
    /// Pure black 2px borders, 0px radius (sharp for clarity), 3px focus ring
    /// Maximum contrast for visibility, clear focus indicators
    /// NO shadows - flat design for accessibility
    /// </summary>
    public static class HighContrastBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // HighContrast: Pure black borders for maximum contrast
            Color blackBorder = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(style);
            Color focusColor = useThemeColors ? theme.AccentColor : StyleColors.GetSelection(style);
            Color borderColor = blackBorder;
            bool showFocusRing = false;

            // HighContrast: Clear state indication for accessibility
            switch (state)
            {
                case ControlState.Hovered:
                    // HighContrast hover: Stay black (clear boundary)
                    borderColor = blackBorder;
                    break;
                case ControlState.Pressed:
                    // HighContrast pressed: Stay black (clear)
                    borderColor = blackBorder;
                    break;
                case ControlState.Selected:
                    // HighContrast selected: Yellow for WCAG AAA visibility
                    borderColor = focusColor; // Yellow (#FFFF00)
                    showFocusRing = true;
                    break;
                case ControlState.Disabled:
                    // HighContrast disabled: Dark gray (still visible)
                    borderColor = Color.FromArgb(100, 100, 100);
                    break;
            }

            // HighContrast focused: CRITICAL 3px focus ring for accessibility
            if (isFocused)
            {
                borderColor = focusColor; // Yellow
                showFocusRing = true;
            }

            // Paint HighContrast 2px black border (0px radius - sharp for clarity)
            float borderWidth = StyleBorders.GetBorderWidth(style); // 2.0f
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // Paint WCAG AAA compliant 3px focus ring (yellow for maximum contrast)
            if (showFocusRing)
            {
                float ringWidth = StyleBorders.GetRingWidth(style); // 3.0f
                float ringOffset = StyleBorders.GetRingOffset(style); // 2.0f
                BorderPainterHelpers.PaintRing(g, path, focusColor, ringWidth, ringOffset);
            }

            // Return the area inside the border
            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}
