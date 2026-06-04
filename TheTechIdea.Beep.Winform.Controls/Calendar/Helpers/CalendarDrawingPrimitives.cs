using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Calendar;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
    /// <summary>
    /// Theme-agnostic drawing primitives shared by all view painters.
    /// These helpers do not read <c>IBeepTheme</c> or <c>BeepControlStyle</c>;
    /// they only provide shape + math building blocks that the view painters
    /// combine with theme colors and style metrics.
    /// </summary>
    public static class CalendarDrawingPrimitives
    {
        /// <summary>Returns a rounded rectangle path. Falls back to a plain rectangle when radius is 0 or the rect is too small.</summary>
        public static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            radius = Math.Max(0, Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2));
            int d = radius * 2;
            var path = new GraphicsPath();
            if (radius == 0 || rect.Width <= 1 || rect.Height <= 1)
            {
                path.AddRectangle(rect);
                return path;
            }
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>Returns a contrasting text color (light or dark) based on the background luminance.</summary>
        public static Color GetContrastingTextColor(Color background)
        {
            double luminance = (0.299 * background.R + 0.587 * background.G + 0.114 * background.B) / 255.0;
            return luminance > 0.58 ? Color.FromArgb(32, 33, 36) : Color.White;
        }

        /// <summary>Linear blend between two colors.</summary>
        public static Color Blend(Color a, Color b, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            int r = (int)Math.Round(a.R + ((b.R - a.R) * amount));
            int g = (int)Math.Round(a.G + ((b.G - a.G) * amount));
            int bch = (int)Math.Round(a.B + ((b.B - a.B) * amount));
            return Color.FromArgb(r, g, bch);
        }

        /// <summary>Scale a base metric by the current density/DPI scale.</summary>
        public static int ScaleMetric(int baseValue, float densityScale)
        {
            var scale = densityScale <= 0 ? 1.0f : densityScale;
            return Math.Max(1, (int)Math.Round(baseValue * scale));
        }

        /// <summary>Look up an event category color from a category list. Falls back to <see cref="Color.Gray"/>.</summary>
        public static Color GetCategoryColor(List<EventCategory> categories, int categoryId)
        {
            if (categories == null) return Color.Gray;
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].Id == categoryId) return categories[i].Color;
            }
            return Color.Gray;
        }

        /// <summary>Pick a contrasting foreground color for a given background.</summary>
        public static Color GetContrastingForeground(Color background, Color darkText, Color lightText)
        {
            return GetContrastingTextColor(background) == Color.White ? lightText : darkText;
        }

        /// <summary>Draws a text string with the supplied foreground color and the supplied font in the supplied rect.</summary>
        public static void DrawText(Graphics g, string text, Font font, Color color, Rectangle rect,
            StringAlignment alignment = StringAlignment.Near,
            StringAlignment lineAlignment = StringAlignment.Near,
            StringTrimming trimming = StringTrimming.None)
        {
            using var brush = new SolidBrush(color);
            using var sf = new StringFormat
            {
                Alignment = alignment,
                LineAlignment = lineAlignment,
                Trimming = trimming
            };
            g.DrawString(text ?? string.Empty, font, brush, rect, sf);
        }
    }
}
