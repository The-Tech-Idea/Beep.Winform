using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Helpers
{
    /// <summary>
    /// Paints Krypton-style docking caption tabs and chrome buttons (pin, float, close).
    /// Uses <see cref="StyledImagePainter"/> for all SVG icons.
    /// </summary>
    internal static class DockingCaptionPainter
    {
        public const int TabIconSize = 16;
        public const int TabIconGap = 4;
        public const int TabTextPadding = 6;
        public const int ButtonInset = 2;

        /// <summary>Krypton order (left to right on strip): pin (auto-hide), float, close.</summary>
        public static class CaptionIcons
        {
            public static string Close => SvgsUIcons.Window.Close;
            public static string DropDown => SvgsUIcons.Carets.Down;
            public static string Float => SvgsUIcons.Window.Maximize;
            // Deliberately empty: the icon set has no plain thumbtack, and fi-tr-map-pin.svg is a
            // location marker, not a pin you press into a board. An empty path makes PaintIcon
            // no-op so PaintPinFallback's drawn thumbtack is what appears.
            public static string Pin => string.Empty;
            public static string DefaultTab => SvgsUIcons.Common.Document;
        }

        public static void PaintIcon(Graphics g, Rectangle bounds, string iconPath, Color tint)
        {
            if (g == null || bounds.IsEmpty || string.IsNullOrWhiteSpace(iconPath))
                return;

            var paintBounds = Inset(bounds, ButtonInset);
            if (paintBounds.Width < 2 || paintBounds.Height < 2)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            try
            {
                StyledImagePainter.PaintWithTint(g, paintBounds, iconPath, tint, 1f, 0);
            }
            catch
            {
                // Caller paints a vector fallback so the docking chrome remains usable.
            }
        }

        public static void PaintTabIcon(Graphics g, Rectangle tabRect, string iconPath, Color tint)
        {
            if (g == null || tabRect.IsEmpty)
                return;

            var iconRect = new Rectangle(
                tabRect.Left + TabTextPadding,
                tabRect.Top + (tabRect.Height - TabIconSize) / 2,
                TabIconSize,
                TabIconSize);

            var resolved = ResolveTabIconPath(iconPath);
            PaintIcon(g, iconRect, resolved, tint);
        }

        public static int GetTabContentLeft(bool hasIcon)
        {
            if (!hasIcon)
                return TabTextPadding;

            return TabTextPadding + TabIconSize + TabIconGap;
        }

        public static bool HasTabIcon(string iconPath) => !string.IsNullOrWhiteSpace(iconPath);

        public static string ResolveTabIconPath(string iconPath)
        {
            if (!string.IsNullOrWhiteSpace(iconPath))
            {
                if (SvgsUIcons.Exists(iconPath))
                    return iconPath;

                if (SvgsUIcons.TryGet(iconPath, out var resolved))
                    return resolved;
            }

            return CaptionIcons.DefaultTab;
        }

        public static void PaintCloseFallback(Graphics g, Rectangle bounds, Color color)
        {
            if (bounds.IsEmpty)
                return;

            var inset = Inset(bounds, ButtonInset + 2);
            using var pen = new Pen(color, 1.75f);
            g.DrawLine(pen, inset.Left, inset.Top, inset.Right, inset.Bottom);
            g.DrawLine(pen, inset.Right, inset.Top, inset.Left, inset.Bottom);
        }

        /// <summary>
        /// Draws the auto-hide affordance as a thumbtack seen side-on: a head bar, a tapering
        /// barrel and a needle.
        /// </summary>
        /// <remarks>
        /// The previous glyph was a filled circle with a line rising out of it, which reads as a
        /// lollipop or a map marker rather than a pin — and <c>CaptionIcons.Pin</c> compounded it by
        /// pointing at <c>fi-tr-map-pin.svg</c>, a location marker. "Pin this panel" means a
        /// thumbtack; a map pin means "a place", which is a different idea entirely.
        /// <para>
        /// Drawn rather than taken from the icon set because the set has no plain thumbtack — only
        /// <c>fi-tr-thumbtack-slash</c>, which is the <i>unpin</i> state. Geometry is proportional
        /// so it stays correct as the button scales with DPI.
        /// </para>
        /// </remarks>
        public static void PaintPinFallback(Graphics g, Rectangle bounds, Color color)
        {
            if (bounds.IsEmpty)
                return;

            var box = Inset(bounds, Math.Max(2, bounds.Width / 6));
            if (box.Width < 6 || box.Height < 6)
                box = bounds;

            int cx = box.Left + box.Width / 2;
            int headTop = box.Top;
            int headHeight = Math.Max(2, box.Height / 5);
            int headHalf = Math.Max(3, box.Width / 2 - 1);

            // The shaft is deliberately much narrower than the head, and parallel-sided. A shaft
            // that merely tapers from the head reads as a funnel: the step is what identifies a
            // thumbtack.
            int barrelTop = headTop + headHeight;
            int barrelBottom = box.Top + (box.Height * 3) / 5;
            int barrelHalf = Math.Max(1, headHalf / 3);

            var prior = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using var brush = new SolidBrush(color);

            // Head: the flat cap the thumb presses.
            g.FillRectangle(brush, cx - headHalf, headTop, headHalf * 2, headHeight);

            // Barrel: tapering toward the needle, which is what makes it read as a pin rather
            // than a nail.
            g.FillRectangle(brush, cx - barrelHalf, barrelTop, barrelHalf * 2, barrelBottom - barrelTop);

            // Needle.
            using var pen = new Pen(color, Math.Max(1f, box.Width / 12f));
            g.DrawLine(pen, cx, barrelBottom, cx, box.Bottom);

            g.SmoothingMode = prior;
        }

        public static void PaintFloatFallback(Graphics g, Rectangle bounds, Color color)
        {
            if (bounds.IsEmpty)
                return;

            var inset = Inset(bounds, ButtonInset + 1);
            using var pen = new Pen(color, 1.5f);
            g.DrawRectangle(pen, inset.Left, inset.Top + 3, inset.Width - 4, inset.Height - 6);
            g.DrawRectangle(pen, inset.Left + 3, inset.Top, inset.Width - 4, inset.Height - 6);
        }

        public static void PaintDropDownFallback(Graphics g, Rectangle bounds, Color color)
        {
            if (bounds.IsEmpty)
                return;

            Point p1 = new Point(bounds.Left + bounds.Width / 2 - 4, bounds.Top + bounds.Height / 2 - 2);
            Point p2 = new Point(bounds.Left + bounds.Width / 2 + 4, bounds.Top + bounds.Height / 2 - 2);
            Point p3 = new Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2 + 4);
            using var brush = new SolidBrush(color);
            g.FillPolygon(brush, new[] { p1, p2, p3 });
        }

        private static Rectangle Inset(Rectangle bounds, int amount)
        {
            if (amount <= 0)
                return bounds;

            return Rectangle.Inflate(bounds, -amount, -amount);
        }
    }
}
