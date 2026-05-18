using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
    internal static class CalendarLayoutGeometry
    {
        public const int MinutesPerDay = 24 * 60;

        public static Rectangle GetColumnRect(Rectangle bounds, int index, int count)
        {
            if (count <= 0 || bounds.Width <= 0)
            {
                return Rectangle.Empty;
            }

            index = Math.Max(0, Math.Min(count - 1, index));
            int left = bounds.Left + (bounds.Width * index / count);
            int right = bounds.Left + (bounds.Width * (index + 1) / count);
            return new Rectangle(left, bounds.Top, Math.Max(0, right - left), bounds.Height);
        }

        public static Rectangle GetRowRect(Rectangle bounds, int index, int count)
        {
            if (count <= 0 || bounds.Height <= 0)
            {
                return Rectangle.Empty;
            }

            index = Math.Max(0, Math.Min(count - 1, index));
            int top = bounds.Top + (bounds.Height * index / count);
            int bottom = bounds.Top + (bounds.Height * (index + 1) / count);
            return new Rectangle(bounds.Left, top, bounds.Width, Math.Max(0, bottom - top));
        }

        public static int GetColumnIndex(Rectangle bounds, int x, int count)
        {
            if (count <= 0 || bounds.Width <= 0 || x < bounds.Left || x >= bounds.Right)
            {
                return -1;
            }

            int relativeX = Math.Max(0, Math.Min(bounds.Width - 1, x - bounds.Left));
            return Math.Max(0, Math.Min(count - 1, relativeX * count / bounds.Width));
        }

        public static int GetRowIndex(Rectangle bounds, int y, int count)
        {
            if (count <= 0 || bounds.Height <= 0 || y < bounds.Top || y >= bounds.Bottom)
            {
                return -1;
            }

            int relativeY = Math.Max(0, Math.Min(bounds.Height - 1, y - bounds.Top));
            return Math.Max(0, Math.Min(count - 1, relativeY * count / bounds.Height));
        }

        public static Rectangle GetTimedArea(Rectangle grid, int timeColumnWidth, int dayHeaderHeight)
        {
            int x = grid.Left + Math.Max(0, timeColumnWidth);
            int y = grid.Top + Math.Max(0, dayHeaderHeight);
            return new Rectangle(
                x,
                y,
                Math.Max(0, grid.Right - x),
                Math.Max(0, grid.Bottom - y));
        }

        public static int GetMinuteFromY(Rectangle timedArea, int y)
        {
            if (timedArea.Height <= 0)
            {
                return 0;
            }

            int relativeY = Math.Max(0, Math.Min(timedArea.Height, y - timedArea.Top));
            return Math.Max(0, Math.Min(MinutesPerDay - 1, relativeY * MinutesPerDay / timedArea.Height));
        }

        public static Rectangle GetTimedEventRect(
            Rectangle dayColumnRect,
            CalendarEvent evt,
            DateTime dayDate,
            int insetX,
            int insetY,
            int minHeight)
        {
            if (evt == null || dayColumnRect.Width <= 0 || dayColumnRect.Height <= 0)
            {
                return Rectangle.Empty;
            }

            double startMinutes = evt.IsAllDay
                ? 0
                : (evt.StartTime - dayDate.Date).TotalMinutes;
            double endMinutes = evt.IsAllDay
                ? MinutesPerDay
                : (evt.EndTime - dayDate.Date).TotalMinutes;

            startMinutes = Math.Max(0, Math.Min(MinutesPerDay, startMinutes));
            endMinutes = Math.Max(0, Math.Min(MinutesPerDay, endMinutes));

            if (endMinutes <= startMinutes)
            {
                double fallbackDuration = Math.Max(30, evt.Duration.TotalMinutes);
                endMinutes = Math.Min(MinutesPerDay, startMinutes + fallbackDuration);
            }

            int top = dayColumnRect.Top + (int)Math.Round(dayColumnRect.Height * startMinutes / MinutesPerDay);
            int bottom = dayColumnRect.Top + (int)Math.Round(dayColumnRect.Height * endMinutes / MinutesPerDay);
            int horizontalInset = Math.Min(Math.Max(0, insetX), Math.Max(0, (dayColumnRect.Width - 1) / 2));
            int verticalInset = Math.Min(Math.Max(0, insetY), Math.Max(0, (dayColumnRect.Height - 1) / 2));
            int x = dayColumnRect.Left + horizontalInset;
            int y = top + verticalInset;
            int width = Math.Max(1, dayColumnRect.Width - (horizontalInset * 2));
            int availableHeight = Math.Max(1, dayColumnRect.Bottom - y - verticalInset);
            int requestedHeight = Math.Max(minHeight, bottom - top - (verticalInset * 2));
            int height = Math.Max(1, Math.Min(availableHeight, requestedHeight));

            return new Rectangle(x, y, width, height);
        }
    }
}
