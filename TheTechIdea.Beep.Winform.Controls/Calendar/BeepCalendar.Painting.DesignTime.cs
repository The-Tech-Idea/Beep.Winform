using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// Renders a blank monthly calendar grid in the Visual Studio Designer.
        /// Shows the real current month with today highlighted so the control looks
        /// like a real calendar rather than a grey placeholder box.
        /// </summary>
        private void PaintDesignTimePlaceholder(Graphics g)
        {
            var bounds = ClientRectangle;
            if (bounds.Width < 10 || bounds.Height < 10)
                return;

            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            // ── palette (Material3 purple) ──────────────────────────────────────
            var bgColor       = Color.FromArgb(255, 255, 255);
            var headerBg      = Color.FromArgb(103, 80, 164);
            var headerFg      = Color.White;
            var dayRowBg      = Color.FromArgb(237, 231, 246);
            var dayRowFg      = Color.FromArgb(103, 80, 164);
            var cellBorder    = Color.FromArgb(228, 228, 228);
            var dayFg         = Color.FromArgb(30, 30, 30);
            var otherMonthFg  = Color.FromArgb(190, 190, 190);
            var todayCircle   = Color.FromArgb(103, 80, 164);
            var todayTextClr  = Color.White;

            // ── layout (DPI-scaled for designer process) ────────────────────────
            int headerH = ScaleMetric(CalendarTokens.HeaderHeight);
            int dayRowH = ScaleMetric(CalendarTokens.DayHeaderHeight);
            const int rows = 6;
            int gridTop   = headerH + dayRowH;
            int cellW     = Math.Max(1, bounds.Width / 7);
            int cellH     = Math.Max(1, (bounds.Height - gridTop) / rows);

            g.SmoothingMode      = SmoothingMode.AntiAlias;
            g.TextRenderingHint  = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // ── background ─────────────────────────────────────────────────────
            using (var br = new SolidBrush(bgColor))
                g.FillRectangle(br, bounds);

            // ── header bar ─────────────────────────────────────────────────────
            var headerRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, headerH);
            using (var br = new SolidBrush(headerBg))
                g.FillRectangle(br, headerRect);

            using (var font = new Font("Segoe UI", 13f, FontStyle.Bold, GraphicsUnit.Point))
            using (var br   = new SolidBrush(headerFg))
            using (var sf   = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                g.DrawString(today.ToString("MMMM yyyy"), font, br, headerRect, sf);

            // ── day-of-week row ────────────────────────────────────────────────
            var dayRowRect = new Rectangle(bounds.X, bounds.Y + headerH, bounds.Width, dayRowH);
            using (var br = new SolidBrush(dayRowBg))
                g.FillRectangle(br, dayRowRect);

            string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            using (var font = new Font("Segoe UI", 8.5f, FontStyle.Bold, GraphicsUnit.Point))
            using (var br   = new SolidBrush(dayRowFg))
            using (var sf   = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                for (int col = 0; col < 7; col++)
                {
                    var r = new Rectangle(bounds.X + col * cellW, bounds.Y + headerH, cellW, dayRowH);
                    g.DrawString(dayNames[col], font, br, r, sf);
                }
            }

            // ── day grid ───────────────────────────────────────────────────────
            int startOffset  = (int)monthStart.DayOfWeek;  // 0 = Sunday
            int daysInMonth  = DateTime.DaysInMonth(today.Year, today.Month);
            var prevMonth    = monthStart.AddMonths(-1);
            int daysInPrev   = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

            using (var borderPen    = new Pen(cellBorder, 1f))
            using (var dayBrush     = new SolidBrush(dayFg))
            using (var otherBrush   = new SolidBrush(otherMonthFg))
            using (var numFont      = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point))
            using (var numFontBold  = new Font("Segoe UI", 9.5f, FontStyle.Bold,    GraphicsUnit.Point))
            using (var rightSf      = new StringFormat { Alignment = StringAlignment.Far,   LineAlignment = StringAlignment.Near })
            using (var centerSf     = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < 7; col++)
                    {
                        int idx    = row * 7 + col;
                        int dayNum = idx - startOffset + 1;

                        bool isCurrent = dayNum >= 1 && dayNum <= daysInMonth;
                        bool isToday   = isCurrent && dayNum == today.Day;

                        int display = isCurrent
                            ? dayNum
                            : (dayNum < 1 ? daysInPrev + dayNum : dayNum - daysInMonth);

                        var cellRect = new Rectangle(
                            bounds.X + col * cellW,
                            bounds.Y + gridTop + row * cellH,
                            cellW, cellH);

                        g.DrawRectangle(borderPen, cellRect);

                        if (isToday)
                        {
                            int todayPad  = ScaleMetric(10);
                            int todayMin  = ScaleMetric(16);
                            int todayOffY = ScaleMetric(4);
                            int d  = Math.Min(cellW, cellH) - todayPad;
                            d      = Math.Max(d, todayMin);
                            int cx = cellRect.X + (cellRect.Width  - d) / 2;
                            int cy = cellRect.Y + todayOffY;
                            using (var todayBr = new SolidBrush(todayCircle))
                                g.FillEllipse(todayBr, cx, cy, d, d);
                            using (var todayFr = new SolidBrush(todayTextClr))
                                g.DrawString(display.ToString(), numFontBold, todayFr,
                                    new Rectangle(cx, cy, d, d), centerSf);
                        }
                        else
                        {
                            var textRect = new Rectangle(cellRect.X, cellRect.Y + ScaleMetric(3), cellRect.Width - ScaleMetric(4), ScaleMetric(18));
                            g.DrawString(display.ToString(), numFont,
                                isCurrent ? dayBrush : otherBrush,
                                textRect, rightSf);
                        }
                    }
                }
            }

            // ── outer border ───────────────────────────────────────────────────
            using (var outerPen = new Pen(_currentTheme?.BorderColor ?? Color.FromArgb(200, 200, 200), 1f))
                g.DrawRectangle(outerPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
        }
    }
}