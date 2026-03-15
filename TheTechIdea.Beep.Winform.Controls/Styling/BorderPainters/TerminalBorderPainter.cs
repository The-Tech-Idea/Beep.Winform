using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Terminal border painter — pixel-crisp frame that follows the passed GraphicsPath
    /// (typically a chamfered rectangle from CreateTerminalPath), with bright corner
    /// accent lines at each chamfer point for the ASCII bracket aesthetic.
    /// </summary>
    public static class TerminalBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            const float borderWidth = 2f;

            // Inset the path so the stroke sits inside the original boundary
            using var insetPath = BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
            GraphicsPath drawTarget = (insetPath != null && insetPath.PointCount > 2) ? insetPath : path;

            RectangleF bounds = drawTarget.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return path;

            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(
                theme, useThemeColors, "Border", Color.FromArgb(0, 255, 0));
            borderColor = BorderPainterHelpers.WithAlpha(borderColor, 200);

            // Brighten on focus
            if (isFocused || state == ControlState.Focused)
                borderColor = BorderPainterHelpers.WithAlpha(borderColor, 255);

            var oldSmooth = g.SmoothingMode;
            var oldPixel = g.PixelOffsetMode;
            g.SmoothingMode = SmoothingMode.None;
            g.PixelOffsetMode = PixelOffsetMode.None;

            // Draw the main border following the path shape (chamfered rect)
            var pen = PaintersFactory.GetPen(borderColor, borderWidth);
            g.DrawPath(pen, drawTarget);

            // Corner accent emphasis — draw slightly brighter/thicker marks
            // at each corner of the bounding rect to evoke ASCII bracket hints
            Rectangle rect = Rectangle.Round(bounds);
            int right = rect.Right - 1;
            int bottom = rect.Bottom - 1;
            int accent = Math.Max(4, Math.Min(8, Math.Min(rect.Width, rect.Height) / 12));

            Color accentColor = BorderPainterHelpers.WithAlpha(borderColor, 255);
            var accentPen = PaintersFactory.GetPen(accentColor, borderWidth + 1f);

            // Top-left accent
            g.DrawLine(accentPen, rect.Left, rect.Top + accent, rect.Left, rect.Top);
            g.DrawLine(accentPen, rect.Left, rect.Top, rect.Left + accent, rect.Top);
            // Top-right accent
            g.DrawLine(accentPen, right - accent, rect.Top, right, rect.Top);
            g.DrawLine(accentPen, right, rect.Top, right, rect.Top + accent);
            // Bottom-left accent
            g.DrawLine(accentPen, rect.Left, bottom - accent, rect.Left, bottom);
            g.DrawLine(accentPen, rect.Left, bottom, rect.Left + accent, bottom);
            // Bottom-right accent
            g.DrawLine(accentPen, right - accent, bottom, right, bottom);
            g.DrawLine(accentPen, right, bottom - accent, right, bottom);

            g.SmoothingMode = oldSmooth;
            g.PixelOffsetMode = oldPixel;
            return path.CreateInsetPath(borderWidth);
        }
    }
}
