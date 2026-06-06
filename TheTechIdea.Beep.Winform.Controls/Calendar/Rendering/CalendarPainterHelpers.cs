using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    /// <summary>
    /// Theme-aware draw helpers shared by every per-view
    /// <see cref="ICalendarViewPainter"/>. Use
    /// <see cref="ViewPaintArgs"/> for the color palette + metrics + fonts.
    /// </summary>
    internal static class CalendarPainterHelpers
    {
        // ── Layout / hit-test primitives ──────────────────────────────────

        public static int GetColumnIndex(Rectangle bounds, int x, int count)
        {
            if (count <= 0 || bounds.Width <= 0 || x < bounds.Left || x >= bounds.Right) return -1;
            int relativeX = Math.Max(0, Math.Min(bounds.Width - 1, x - bounds.Left));
            return Math.Max(0, Math.Min(count - 1, relativeX * count / bounds.Width));
        }

        public static int GetRowIndex(Rectangle bounds, int y, int count)
        {
            if (count <= 0 || bounds.Height <= 0 || y < bounds.Top || y >= bounds.Bottom) return -1;
            int relativeY = Math.Max(0, Math.Min(bounds.Height - 1, y - bounds.Top));
            return Math.Max(0, Math.Min(count - 1, relativeY * count / bounds.Height));
        }

        public static Rectangle GetColumnRect(Rectangle bounds, int index, int count)
        {
            if (count <= 0 || bounds.Width <= 0) return Rectangle.Empty;
            index = Math.Max(0, Math.Min(count - 1, index));
            int left = bounds.Left + (bounds.Width * index / count);
            int right = bounds.Left + (bounds.Width * (index + 1) / count);
            return new Rectangle(left, bounds.Top, Math.Max(0, right - left), bounds.Height);
        }

        public static Rectangle GetRowRect(Rectangle bounds, int index, int count)
        {
            if (count <= 0 || bounds.Height <= 0) return Rectangle.Empty;
            index = Math.Max(0, Math.Min(count - 1, index));
            int top = bounds.Top + (bounds.Height * index / count);
            int bottom = bounds.Top + (bounds.Height * (index + 1) / count);
            return new Rectangle(bounds.Left, top, bounds.Width, Math.Max(0, bottom - top));
        }

        public static int GetMinuteFromY(Rectangle timedArea, int y)
        {
            if (timedArea.Height <= 0) return 0;
            int relativeY = Math.Max(0, Math.Min(timedArea.Height, y - timedArea.Top));
            const int MinutesPerDay = 24 * 60;
            return Math.Max(0, Math.Min(MinutesPerDay - 1, relativeY * MinutesPerDay / timedArea.Height));
        }

        public static Rectangle GetTimedEventRect(Rectangle dayColumnRect, CalendarEvent evt, DateTime dayDate,
            int eventInsetX, int eventInsetY, int minEventHitHeight)
        {
            if (evt == null || dayColumnRect.Width <= 0 || dayColumnRect.Height <= 0) return Rectangle.Empty;
            const int MinutesPerDay = 24 * 60;
            double startMinutes = evt.IsAllDay ? 0 : (evt.StartTime - dayDate.Date).TotalMinutes;
            double endMinutes = evt.IsAllDay ? MinutesPerDay : (evt.EndTime - dayDate.Date).TotalMinutes;
            startMinutes = Math.Max(0, Math.Min(MinutesPerDay, startMinutes));
            endMinutes = Math.Max(0, Math.Min(MinutesPerDay, endMinutes));
            if (endMinutes <= startMinutes)
            {
                double fallbackDuration = Math.Max(30, evt.Duration.TotalMinutes);
                endMinutes = Math.Min(MinutesPerDay, startMinutes + fallbackDuration);
            }
            int top = dayColumnRect.Top + (int)Math.Round(dayColumnRect.Height * startMinutes / MinutesPerDay);
            int bottom = dayColumnRect.Top + (int)Math.Round(dayColumnRect.Height * endMinutes / MinutesPerDay);
            int horizontalInset = Math.Min(Math.Max(0, eventInsetX), Math.Max(0, (dayColumnRect.Width - 1) / 2));
            int verticalInset = Math.Min(Math.Max(0, eventInsetY), Math.Max(0, (dayColumnRect.Height - 1) / 2));
            int x = dayColumnRect.Left + horizontalInset;
            int y = top + verticalInset;
            int width = Math.Max(1, dayColumnRect.Width - (horizontalInset * 2));
            int availableHeight = Math.Max(1, dayColumnRect.Bottom - y - verticalInset);
            int requestedHeight = Math.Max(minEventHitHeight, bottom - top - (verticalInset * 2));
            int height = Math.Max(1, Math.Min(availableHeight, requestedHeight));
            return new Rectangle(x, y, width, height);
        }

        public static CalendarEventResizeEdge ResolveResizeEdge(Point location, Rectangle eventRect, int resizeHandleSizePx)
        {
            if (eventRect.Height <= 0) return CalendarEventResizeEdge.None;
            if (location.Y <= eventRect.Top + resizeHandleSizePx) return CalendarEventResizeEdge.Start;
            if (location.Y >= eventRect.Bottom - resizeHandleSizePx) return CalendarEventResizeEdge.End;
            return CalendarEventResizeEdge.None;
        }

        // ── Paint primitives ──────────────────────────────────────────────

        public static void FillRoundedRect(Graphics g, Rectangle rect, int radius, Color color)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            using var path = CalendarDrawingPrimitives.RoundedRect(rect, Math.Max(0, radius));
            using var brush = new SolidBrush(color);
            g.FillPath(brush, path);
        }

        public static void StrokeRoundedRect(Graphics g, Rectangle rect, int radius, Color color, float width = 1f)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            using var path = CalendarDrawingPrimitives.RoundedRect(rect, Math.Max(0, radius));
            using var pen = new Pen(color, width);
            g.DrawPath(pen, path);
        }

        public static void DrawText(Graphics g, string text, Font font, Color color, Rectangle rect,
            StringAlignment horizontal = StringAlignment.Near, StringAlignment vertical = StringAlignment.Near,
            StringTrimming trimming = StringTrimming.EllipsisCharacter, bool centerVertically = true)
        {
            if (string.IsNullOrEmpty(text) || font == null) return;
            if (rect.Width <= 0 || rect.Height <= 0) return;
            using var brush = new SolidBrush(color);
            using var sf = new StringFormat
            {
                Alignment = horizontal,
                LineAlignment = centerVertically ? StringAlignment.Center : vertical,
                Trimming = trimming,
                FormatFlags = StringFormatFlags.NoWrap
            };
            g.DrawString(text, font, brush, rect, sf);
        }

        public static void DrawImageIcon(Graphics g, Image icon, Rectangle rect)
        {
            if (icon == null) return;
            if (rect.Width <= 0 || rect.Height <= 0) return;
            g.DrawImage(icon, rect);
        }

        public static Image LoadIcon(string resourceName, TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle style)
        {
            if (string.IsNullOrEmpty(resourceName)) return null;
            try
            {
                using var bmp = new Bitmap(64, 64);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using var path = new GraphicsPath();
                    path.AddRectangle(new RectangleF(2, 2, 60, 60));
                    StyledImagePainter.Paint(g, path, resourceName, style);
                }
                return bmp.Clone() as Image;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Paint an SVG icon resource into the target rect using
        /// <see cref="StyledImagePainter"/>. The image path is the resource
        /// name returned by <c>SvgsUIcons.Common.*</c> etc. The painter's
        /// internal cache avoids re-decoding the SVG on every redraw.
        /// </summary>
        public static void PaintIcon(Graphics g, Rectangle rect, string resourcePath, ViewPaintArgs args)
        {
            if (string.IsNullOrEmpty(resourcePath) || rect.Width <= 0 || rect.Height <= 0) return;
            try
            {
                StyledImagePainter.Paint(g, rect, resourcePath, args.ControlStyle);
            }
            catch
            {
                /* icon paint failures are non-fatal */
            }
        }

        /// <summary>
        /// W8 - fetch the cached IBeepUIComponent for the supplied cell key
        /// (via the calendar's <c>CellComponentFactory</c>) and call
        /// its <c>Draw(g, rect)</c>. Returns true when a component was
        /// actually drawn; false when no factory is registered (or the
        /// factory returned null), so the caller can fall back to its
        /// default rendering. Centralizes the try/catch boundary so the
        /// rest of every painter stays linear.
        /// </summary>
        public static bool TryDrawCellComponent(Graphics g, Rectangle rect, string cellKey,
            CalendarCellContext ctx, ViewPaintArgs args)
        {
            if (args?.Owner == null) return false;
            try
            {
                var comp = args.Owner.GetCellComponent(cellKey, ctx);
                if (comp == null) return false;
                if (comp is Control asControl)
                {
                    if (asControl.Parent == null)
                    {
                        asControl.Bounds = rect;
                        if (!asControl.Visible) asControl.Visible = true;
                    }
                    if (asControl.Parent != null)
                        return true;
                }
                comp.Draw(g, rect);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
