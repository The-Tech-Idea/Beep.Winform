using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// Static helper class providing common rendering utilities for all card painters
    /// </summary>
    internal static class CardRenderingHelpers
    {
        // Simple font cache keyed by family+size+style
        private static readonly Dictionary<string, Font> _fontCache = new Dictionary<string, Font>();
        private static Font GetCachedFont(FontFamily family, float size, FontStyle style)
        {
            string key = $"{family.Name}-{size}-{(int)style}";
            if (_fontCache.TryGetValue(key, out var f)) return f;
            try
            {
                f = new Font(family, size, style);
            }
            catch
            {
                f = SystemFonts.DefaultFont;
            }
            _fontCache[key] = f;
            return f;
        }

        /// <summary>
        /// Draws chip/tag elements in a horizontal row
        /// </summary>
        public static void DrawChips(Graphics g, BaseControl owner, Rectangle area, Color accent, IEnumerable<string> tags)
        {
            if (tags == null || !tags.Any()) return;

            int x = area.Left, y = area.Top, h = Math.Min(24, area.Height);
            var font = GetCachedFont(owner.Font.FontFamily,8.5f, FontStyle.Regular);

            foreach (var tag in tags)
            {
                if (string.IsNullOrWhiteSpace(tag)) continue;

                var textSize = TextUtils.MeasureText(g, tag, font);
                int w = Math.Min(120, (int)(textSize.Width +16));
                if (x + w > area.Right -8) break;

                var chipRect = new Rectangle(x, y, w, h);
                using var chipPath = CreateRoundedPath(chipRect, h /2);
                var chipBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(20, accent));
                var chipPen = PaintersFactory.GetPen(Color.FromArgb(60, accent),1);
                var textBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(180, Color.Black));

                g.FillPath(chipBrush, chipPath);
                g.DrawPath(chipPen, chipPath);

                var textRect = new Rectangle(chipRect.X +8, chipRect.Y, chipRect.Width -16, chipRect.Height);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(tag, font, textBrush, textRect, format);

                x += w +8;
            }
        }

        /// <summary>
        /// Draws a pill-shaped badge with text
        /// </summary>
        public static void DrawBadge(Graphics g, Rectangle rect, string text, Color backColor, Color foreColor, Font font)
        {
            if (string.IsNullOrEmpty(text) || rect.IsEmpty) return;

            using var badgePath = CreateRoundedPath(rect, rect.Height /2);
            var badgeBrush = PaintersFactory.GetSolidBrush(backColor);
            var textBrush = PaintersFactory.GetSolidBrush(foreColor);

            g.FillPath(badgeBrush, badgePath);

            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(text, font, textBrush, rect, format);
        }

        /// <summary>
        /// Draws a rating as filled stars
        /// </summary>
        public static void DrawStars(Graphics g, Rectangle rect, int rating, Color color)
        {
            if (rating <=0 || rect.IsEmpty) return;

            int starSize = Math.Min(16, rect.Height);
            int totalWidth = rating * starSize + (rating -1) *2;
            int startX = rect.X + (rect.Width - totalWidth) /2;

            var starBrush = PaintersFactory.GetSolidBrush(color);

            for (int i =0; i < rating; i++)
            {
                var starRect = new Rectangle(startX + i * (starSize +2), rect.Y + (rect.Height - starSize) /2, starSize, starSize);
                DrawStar(g, starBrush, starRect);
            }
        }

        /// <summary>
        /// Draws a single filled star
        /// </summary>
        private static void DrawStar(Graphics g, Brush brush, Rectangle rect)
        {
            var points = new PointF[10];
            var center = new PointF(rect.X + rect.Width /2f, rect.Y + rect.Height /2f);
            var outerRadius = rect.Width /2f;
            var innerRadius = outerRadius *0.4f;

            for (int i =0; i <10; i++)
            {
                var angle = i * Math.PI /5 - Math.PI /2;
                var radius = (i %2 ==0) ? outerRadius : innerRadius;
                points[i] = new PointF(
                    center.X + (float)(Math.Cos(angle) * radius),
                    center.Y + (float)(Math.Sin(angle) * radius)
                );
            }

            g.FillPolygon(brush, points);
        }

        /// <summary>
        /// Draws a status indicator with dot and text
        /// </summary>
        public static void DrawStatus(Graphics g, Rectangle rect, string text, Color color, Font font)
        {
            if (string.IsNullOrEmpty(text) || rect.IsEmpty) return;

            // Draw status dot
            int dotSize =8;
            var dotRect = new Rectangle(rect.X, rect.Y + (rect.Height - dotSize) /2, dotSize, dotSize);
            var dotBrush = PaintersFactory.GetSolidBrush(color);
            g.FillEllipse(dotBrush, dotRect);

            // Draw status text
            var textRect = new Rectangle(dotRect.Right +6, rect.Y, rect.Width - dotSize -6, rect.Height);
            var textBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(120, Color.Black));
            var format = new StringFormat { LineAlignment = StringAlignment.Center };
            g.DrawString(text, font, textBrush, textRect, format);
        }

        /// <summary>
        /// Creates a rounded rectangle graphics path
        /// </summary>
        public static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <=0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int d = radius *2;
            path.AddArc(rect.Left, rect.Top, d, d,180,90);
            path.AddArc(rect.Right - d, rect.Top, d, d,270,90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d,0,90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d,90,90);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Draws a multi-layer soft shadow effect
        /// </summary>
        public static void DrawSoftShadow(Graphics g, Rectangle rect, int radius, int layers =6, int offset =3)
        {
            for (int i = layers; i >0; i--)
            {
                int spread = i *2;
                int alpha = (int)(15 * (i / (float)layers));
                var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(alpha, Color.Black));
                var shadowRect = new Rectangle(
                    rect.X + offset - spread /2,
                    rect.Y + offset - spread /2,
                    rect.Width + spread,
                    rect.Height + spread
                );
                using var shadowPath = CreateRoundedPath(shadowRect, radius + i);
                g.FillPath(shadowBrush, shadowPath);
            }
        }
    }
}
