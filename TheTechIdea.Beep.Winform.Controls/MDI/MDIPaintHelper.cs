using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.MDI
{
    internal class MDIPaintHelper
    {
        private readonly IBeepTheme _theme;
        public MDIPaintHelper(IBeepTheme theme) => _theme = theme;

        public void DrawTab(Graphics g, MDIDocument doc, Rectangle bounds, string title, Font font, bool isActive, bool isHovered, bool showClose, bool isCloseHovered, float anim,
            Color inactiveBack, Color inactiveFore, Color activeBack, Color activeFore, Color hoverBack, Color borderColor)
        {
            if (bounds.IsEmpty) return;
            var back = isActive ? LerpColor(inactiveBack, activeBack, 0.6f + anim * 0.4f) : (isHovered ? hoverBack : inactiveBack);
            using var path = RoundedRect(bounds, 6);
            using (var b = new SolidBrush(back)) g.FillPath(b, path);
            using (var pen = new Pen(borderColor)) g.DrawPath(pen, path);

            var fore = isActive ? activeFore : inactiveFore;

            // Icon
            int textLeft = bounds.Left + 8;
            if (doc.Icon != null)
            {
                var iconRect = new Rectangle(textLeft, bounds.Top + (bounds.Height - 16) / 2, 16, 16);
                g.DrawImage(doc.Icon, iconRect);
                textLeft = iconRect.Right + 4;
            }

            // Title
            int closeSpace = showClose ? 20 : 4;
            int pinSpace = doc.IsPinned ? 14 : 0;
            var textRect = new Rectangle(textLeft, bounds.Y, bounds.Width - (textLeft - bounds.Left) - closeSpace - pinSpace, bounds.Height);
            TextRenderer.DrawText(g, title + (doc.IsDirty ? " *" : string.Empty), font, textRect, fore, TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);

            // Pin glyph
            if (doc.IsPinned)
            {
                var pinRect = new Rectangle(bounds.Right - (showClose ? 34 : 18), bounds.Y + (bounds.Height - 14) / 2, 14, 14);
                DrawPinGlyph(g, pinRect, fore);
            }

            // Close
            if (showClose && doc.CanClose)
            {
                var closeRect = new Rectangle(bounds.Right - 18, bounds.Y + (bounds.Height - 14) / 2, 14, 14);
                DrawCloseGlyph(g, closeRect, isCloseHovered ? activeFore : Color.FromArgb(160, fore));
            }
        }

        public void DrawScrollButton(Graphics g, Rectangle rect, ScrollButtonType type, Color back, Color fore, Color border)
        {
            if (rect.IsEmpty) return;
            using var b = new SolidBrush(back);
            using var p = new Pen(border);
            g.FillRectangle(b, rect);
            g.DrawRectangle(p, rect);
            Point center = new(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            Point[] pts = type == ScrollButtonType.Left
                ? new[] { new Point(center.X + 3, center.Y - 5), new Point(center.X - 2, center.Y), new Point(center.X + 3, center.Y + 5) }
                : new[] { new Point(center.X - 3, center.Y - 5), new Point(center.X + 2, center.Y), new Point(center.X - 3, center.Y + 5) };
            using var bFore = new SolidBrush(fore);
            g.FillPolygon(bFore, pts);
        }

        public void DrawNewDocumentButton(Graphics g, Rectangle rect, Color back, Color fore, Color border)
        {
            if (rect.IsEmpty) return;
            using var b = new SolidBrush(back);
            using var p = new Pen(border);
            g.FillRectangle(b, rect);
            g.DrawRectangle(p, rect);
            using var p2 = new Pen(fore, 2);
            int cx = rect.X + rect.Width / 2;
            int cy = rect.Y + rect.Height / 2;
            g.DrawLine(p2, cx - 5, cy, cx + 5, cy);
            g.DrawLine(p2, cx, cy - 5, cx, cy + 5);
        }

        private void DrawCloseGlyph(Graphics g, Rectangle rect, Color color)
        {
            using var p = new Pen(color, 2);
            var old = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawLine(p, rect.X + 3, rect.Y + 3, rect.Right - 3, rect.Bottom - 3);
            g.DrawLine(p, rect.Right - 3, rect.Y + 3, rect.X + 3, rect.Bottom - 3);
            g.SmoothingMode = old;
        }

        private void DrawPinGlyph(Graphics g, Rectangle rect, Color color)
        {
            using var p = new Pen(color, 1.8f);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int cx = rect.X + rect.Width / 2;
            int top = rect.Top + 2;
            int bottom = rect.Bottom - 3;
            g.DrawLine(p, cx, top, cx, bottom);
            g.DrawEllipse(p, cx - 4, top + 2, 8, 8);
            g.DrawLine(p, cx - 4, top + 6, cx + 4, top + 6);
            g.SmoothingMode = SmoothingMode.None;
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            if (d > bounds.Height) d = bounds.Height;
            if (d > bounds.Width) d = bounds.Width;
            var arc = new Rectangle(bounds.Location, new Size(d, d));
            path.AddArc(arc, 180, 90); // TL
            arc.X = bounds.Right - d; path.AddArc(arc, 270, 90); // TR
            arc.Y = bounds.Bottom - d; path.AddArc(arc, 0, 90); // BR
            arc.X = bounds.Left; path.AddArc(arc, 90, 90); // BL
            path.CloseFigure();
            return path;
        }

        private static Color LerpColor(Color a, Color b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return Color.FromArgb((int)(a.A + (b.A - a.A) * t), (int)(a.R + (b.R - a.R) * t), (int)(a.G + (b.G - a.G) * t), (int)(a.B + (b.B - a.B) * t));
        }
    }
}
