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
    /// NeoBrutalist border painter - THICK BLACK BORDERS (signature!)
    /// 4px thick borders, 0px radius (sharp edges), pure black
    /// Neo-Brutalism: Aggressive, raw, brutalist aesthetic
    /// NO subtle transitions - bold on/off states only
    /// </summary>
    public static class NeoBrutalistBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // NeoBrutalist: THICK black borders (signature!)
            Color blackBorder = useThemeColors && theme != null ? theme.BorderColor : StyleColors.GetBorder(style);
            Color borderColor = blackBorder;

            // NeoBrutalist: BOLD state changes (no subtlety!)
            switch (state)
            {
                case ControlState.Hovered:
                    // NeoBrutalist hover: Stay black (bold, no change)
                    borderColor = blackBorder;
                    break;
                case ControlState.Pressed:
                    // NeoBrutalist pressed: Stay black (aggressive)
                    borderColor = blackBorder;
                    break;
                case ControlState.Selected:
                    // NeoBrutalist selected: Stay black (bold)
                    borderColor = blackBorder;
                    break;
                case ControlState.Disabled:
                    // NeoBrutalist disabled: Dark gray (still visible)
                    borderColor = Color.FromArgb(120, 120, 120);
                    break;
            }

            // NeoBrutalist focused: Stay black (no special focus ring - raw design)
            // The thick border itself IS the focus indicator

            // Respect configured border width for NeoBrutalist style - if zero, do not draw border
            float configuredWidth = StyleBorders.GetBorderWidth(style);
            if (configuredWidth <= 0f) return path;
            // Paint NeoBrutalist SIGNATURE 6px THICK black border (0px radius - sharp!)
            float borderWidth = configuredWidth;
            borderColor = BorderPainterHelpers.EnsureVisibleBorderColor(borderColor, theme, state);

            var savedPixel = g.PixelOffsetMode;
            g.PixelOffsetMode = PixelOffsetMode.None;
            using (var pen = new Pen(borderColor, borderWidth))
            {
                pen.LineJoin = LineJoin.Miter;
                pen.Alignment = PenAlignment.Inset;
                g.DrawPath(pen, path);
            }
            g.PixelOffsetMode = savedPixel;

            // Return the area inside the THICK border
            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}
