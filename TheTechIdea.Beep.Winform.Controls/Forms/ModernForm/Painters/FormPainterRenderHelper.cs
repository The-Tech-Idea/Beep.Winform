using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Shared rendering helpers for form painters and caption regions.
    /// Keeps drawing logic out of BeepiFormPro core.
    /// </summary>
    internal static class FormPainterRenderHelper
    {
        /// <summary>
        /// Draw a system button (minimize/maximize/close) with optional hover outline and symbol.
        /// Colors are provided by caller (from FormPainterMetrics) to avoid coupling.
        /// </summary>
        public static void DrawSystemButton(Graphics g, Rectangle bounds, string symbol, bool isHover,
            bool isClose, Font baseFont, Color foregroundColor, Color hoverColor, Color? closeOutlineColor = null)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Hover indicator: outline around the button instead of background fill
            if (isHover)
            {
                var outlineColor = isClose ? (closeOutlineColor ?? Color.FromArgb(232, 17, 35)) : hoverColor; // Windows red default for close
                DrawHoverOutlineRect(g, bounds, outlineColor, 2, 6);
            }

            using var font = new Font(baseFont.FontFamily, baseFont.Size + 2, FontStyle.Regular);
            TextRenderer.DrawText(g, symbol, font, bounds, foregroundColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        /// <summary>
        /// Draw a rounded-rectangle outline for hover/pressed states.
        /// </summary>
        public static void DrawHoverOutlineRect(Graphics g, Rectangle bounds, Color color, int thickness = 2, int cornerRadius = 6)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            int inset = Math.Max(1, thickness);
            var rect = new Rectangle(bounds.X + inset, bounds.Y + inset, bounds.Width - inset * 2, bounds.Height - inset * 2);
            using var pen = new Pen(color, thickness);
            var old = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.DrawPath(pen, path);
            }
            g.SmoothingMode = old;
        }

        /// <summary>
        /// Draw a circular outline for hover/pressed states.
        /// </summary>
        public static void DrawHoverOutlineCircle(Graphics g, Rectangle bounds, Color color, int thickness = 2, int padding = 6)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            int size = Math.Min(bounds.Width, bounds.Height) - padding * 2;
            if (size <= 0) return;
            int cx = bounds.Left + (bounds.Width - size) / 2;
            int cy = bounds.Top + (bounds.Height - size) / 2;
            using var pen = new Pen(color, thickness);
            var old = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawEllipse(pen, cx, cy, size, size);
            g.SmoothingMode = old;
        }

        private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                path.CloseFigure();
                return path;
            }

            // Top-left
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            // Top-right
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            // Bottom-right
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            // Bottom-left
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
