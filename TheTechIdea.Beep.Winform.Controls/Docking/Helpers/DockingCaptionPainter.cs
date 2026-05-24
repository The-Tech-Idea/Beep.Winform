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
            public static string Pin => SvgsUIcons.Map.Pin;
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

        public static bool HasTabIcon(string iconPath) => true;

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

        public static void PaintPinFallback(Graphics g, Rectangle bounds, Color color)
        {
            if (bounds.IsEmpty)
                return;

            var c = new Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2 + 2);
            using var brush = new SolidBrush(color);
            g.FillEllipse(brush, c.X - 3, c.Y - 1, 6, 6);
            using var pen = new Pen(color, 1.5f);
            g.DrawLine(pen, c.X, bounds.Top + 2, c.X, c.Y - 1);
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
