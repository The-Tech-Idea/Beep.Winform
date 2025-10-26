using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Terminal border painter - pixel crisp rectangular frame with ASCII corner hints.
    /// </summary>
    public static class TerminalBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Rectangle rect = Rectangle.Round(path.GetBounds());
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(0, 255, 0));
            borderColor = BorderPainterHelpers.WithAlpha(borderColor, 200);

            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            using (var pen = new Pen(borderColor, 2))
            {
                g.DrawRectangle(pen, rect);
            }

            int corner = 6;
            using (var pen = new Pen(borderColor, 2))
            {
                // Top-left corner (┌)
                g.DrawLine(pen, rect.Left, rect.Top + corner, rect.Left, rect.Top);
                g.DrawLine(pen, rect.Left, rect.Top, rect.Left + corner, rect.Top);

                // Top-right (┐)
                g.DrawLine(pen, rect.Right - corner, rect.Top, rect.Right, rect.Top);
                g.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Top + corner);

                // Bottom-left (└)
                g.DrawLine(pen, rect.Left, rect.Bottom - corner, rect.Left, rect.Bottom);
                g.DrawLine(pen, rect.Left, rect.Bottom, rect.Left + corner, rect.Bottom);

                // Bottom-right (┘)
                g.DrawLine(pen, rect.Right - corner, rect.Bottom, rect.Right, rect.Bottom);
                g.DrawLine(pen, rect.Right, rect.Bottom - corner, rect.Right, rect.Bottom);
            }

            g.SmoothingMode = oldMode;
            return path.CreateInsetPath(2f);
        }
    }
}
