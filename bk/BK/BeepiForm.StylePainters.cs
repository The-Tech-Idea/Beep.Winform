using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls
{
    // BeepiForm partial: simple, style-specific painters using paths only
    public partial class BeepiForm
    {
        // Paint a simple caption band for the current style (path-only, no Rectangle)
        private void PaintStyleCaption(Graphics g)
        {
            if (g == null) return;
            if (_captionHelper == null || !_captionHelper.ShowCaptionBar) return;
            if (WindowState == System.Windows.Forms.FormWindowState.Maximized) return; // keep OS look when maximized

            int w = Math.Max(0, Width);
            int h = Math.Max(0, Height);
            int capH = Math.Max(0, _captionHelper?.CaptionHeight ?? _legacyCaptionHeight);
            if (w <= 0 || capH <= 0) return;

            // Style mapping: keep it simple and clean. Use BackColor (no gradients) for all styles by default.
            // Individual styles can tweak via a small shade if needed.
            Color bandColor = ResolveStyleCaptionColor();

            using var captionPath = BuildRectPath(0, 0, w, capH);
            using var brush = new SolidBrush(bandColor);
            g.FillPath(brush, captionPath);
        }

        // Paint the border for the current style (path-only)
        private void PaintStyleBorder(Graphics g, GraphicsPath windowPath)
        {
            if (g == null || windowPath == null) return;
            if (_borderThickness <= 0) return;
            if (WindowState == System.Windows.Forms.FormWindowState.Maximized) return;

            Color color = ResolveStyleBorderColor();
            if (color == Color.Empty || color == Color.Transparent) return;

            using var pen = new Pen(color, _borderThickness)
            {
                Alignment = PenAlignment.Inset,
                LineJoin = LineJoin.Round
            };
            g.DrawPath(pen, windowPath);
        }

        private Color ResolveStyleCaptionColor()
        {
            // Keep caption band uniform and simple to avoid artifacts or tints
            // Optionally adjust per style if needed
            switch (_formStyle)
            {
                case BeepFormStyle.Modern:
                case BeepFormStyle.Minimal:
                case BeepFormStyle.Material:
                case BeepFormStyle.Office:
                case BeepFormStyle.Classic:
                default:
                    return BackColor;
            }
        }

        private static int ClampByte(int v) => v < 0 ? 0 : (v > 255 ? 255 : v);
        private static Color Darken(Color c, int delta)
            => Color.FromArgb(c.A, ClampByte(c.R - delta), ClampByte(c.G - delta), ClampByte(c.B - delta));

        private Color ResolveStyleBorderColor()
        {
            // If explicit BorderColor was set by the user, honor it
            if (_borderColor != Color.Empty && _borderColor != Color.Transparent)
                return _borderColor;

            // Otherwise derive a neutral border from BackColor to avoid any style/theme purples
            // Slightly darker shade for contrast; tweak per style if desired
            return _formStyle switch
            {
                BeepFormStyle.Modern => Darken(BackColor, 60),
                BeepFormStyle.Minimal => Darken(BackColor, 80),
                BeepFormStyle.Material => Darken(BackColor, 70),
                BeepFormStyle.Office => Darken(BackColor, 50),
                BeepFormStyle.Classic => SystemColors.ActiveBorder,
                _ => Darken(BackColor, 60)
            };
        }
    }
}
