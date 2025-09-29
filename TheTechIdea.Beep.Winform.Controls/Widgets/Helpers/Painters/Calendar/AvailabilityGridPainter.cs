using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// AvailabilityGrid - Availability/booking grid painter with hit areas and hover states
    /// </summary>
    internal sealed class AvailabilityGridPainter : WidgetPainterBase
    {
        private readonly List<(Rectangle rect, int day, int slot)> _slotRects = new();
        private readonly List<(Rectangle rect, int day)> _dayHeaderRects = new();
        private Rectangle _gridRectCache;

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);
            _gridRectCache = ctx.DrawingRect;
            _slotRects.Clear();
            _dayHeaderRects.Clear();
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw availability grid with time slots and booking status
            DrawTimeSlots(g, ctx);
            DrawAvailabilitySlots(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) 
        {
            // Draw grid lines and selection highlights
            DrawGridLines(g, ctx);
            DrawDayHeaders(g, ctx);
        }

        private void DrawTimeSlots(Graphics g, WidgetContext ctx)
        {
            // Draw time slot headers (9 AM, 10 AM, etc.)
            int slotHeight = 30;
            int slotsCount = Math.Max(1, ctx.DrawingRect.Height / slotHeight);

            using var timeFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);
            using var timeBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Theme?.ForeColor ?? Color.Black);

            for (int i = 0; i < slotsCount; i++)
            {
                var timeSlotRect = new Rectangle(
                    ctx.DrawingRect.Left + 4,
                    ctx.DrawingRect.Top + i * slotHeight + 4,
                    60,
                    slotHeight - 2
                );

                // Draw time label
                string timeText = $"{9 + i}:00";
                g.DrawString(timeText, timeFont, timeBrush, timeSlotRect, 
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private void DrawAvailabilitySlots(Graphics g, WidgetContext ctx)
        {
            // Draw availability slots for different days/resources
            int slotHeight = 30;
            int slotWidth = Math.Max(40, (ctx.DrawingRect.Width - 70) / 7); // 7 days
            int slotsCount = Math.Max(1, ctx.DrawingRect.Height / slotHeight);

            // Sample availability data
            var random = new Random(42); // Fixed seed for consistent display

            _slotRects.Clear();
            for (int day = 0; day < 7; day++)
            {
                for (int timeSlot = 0; timeSlot < slotsCount; timeSlot++)
                {
                    var slotRect = new Rectangle(
                        ctx.DrawingRect.Left + 70 + day * slotWidth,
                        ctx.DrawingRect.Top + timeSlot * slotHeight + 4,
                        slotWidth - 2,
                        slotHeight - 2
                    );
                    _slotRects.Add((slotRect, day, timeSlot));

                    // Determine slot status (available, busy, unavailable)
                    int status = random.Next(0, 3);
                    Color slotColor = status switch
                    {
                        0 => Color.FromArgb(200, 76, 175, 80),  // Available (green)
                        1 => Color.FromArgb(200, 255, 193, 7),   // Busy (amber)
                        _ => Color.FromArgb(200, 244, 67, 54)    // Unavailable (red)
                    };

                    using var slotBrush = new SolidBrush(slotColor);
                    g.FillRectangle(slotBrush, slotRect);

                    // Hover overlay
                    if (IsAreaHovered($"Availability_Slot_{day}_{timeSlot}"))
                    {
                        using var hover = new SolidBrush(Color.FromArgb(10, Theme?.PrimaryColor ?? Color.Blue));
                        g.FillRectangle(hover, Rectangle.Inflate(slotRect, 2, 1));
                    }

                    // Add status text if slot is large enough
                    if (slotRect.Width > 30 && slotRect.Height > 20)
                    {
                        string statusText = status switch
                        {
                            0 => "Free",
                            1 => "Busy", 
                            _ => "N/A"
                        };

                        using var statusFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Regular);
                        using var statusBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
                        g.DrawString(statusText, statusFont, statusBrush, slotRect,
                                   new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                }
            }
        }

        private void DrawGridLines(Graphics g, WidgetContext ctx)
        {
            // Draw subtle grid lines
            using var gridPen = new Pen(Color.FromArgb(60, Theme?.BorderColor ?? Color.Gray), 1);
            
            // Horizontal lines
            int slotHeight = 30;
            int slotsCount = Math.Max(1, ctx.DrawingRect.Height / slotHeight);
            
            for (int i = 0; i <= slotsCount; i++)
            {
                int y = ctx.DrawingRect.Top + i * slotHeight;
                g.DrawLine(gridPen, ctx.DrawingRect.Left + 70, y, ctx.DrawingRect.Right, y);
            }

            // Vertical lines
            int slotWidth = Math.Max(40, (ctx.DrawingRect.Width - 70) / 7);
            for (int i = 0; i <= 7; i++)
            {
                int x = ctx.DrawingRect.Left + 70 + i * slotWidth;
                g.DrawLine(gridPen, x, ctx.DrawingRect.Top, x, ctx.DrawingRect.Bottom);
            }
        }

        private void DrawDayHeaders(Graphics g, WidgetContext ctx)
        {
            // Draw day labels at the top
            int slotWidth = Math.Max(40, (ctx.DrawingRect.Width - 70) / 7);
            string[] dayNames = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

            using var dayFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Bold);
            using var dayBrush = new SolidBrush(Theme?.AccentColor ?? Color.Blue);

            _dayHeaderRects.Clear();
            for (int i = 0; i < dayNames.Length && i < 7; i++)
            {
                var dayRect = new Rectangle(
                    ctx.DrawingRect.Left + 70 + i * slotWidth,
                    ctx.DrawingRect.Top - 20,
                    slotWidth,
                    18
                );
                _dayHeaderRects.Add((dayRect, i));

                g.DrawString(dayNames[i], dayFont, dayBrush, dayRect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            // Slots
            foreach (var (rect, day, slot) in _slotRects)
            {
                string name = $"Availability_Slot_{day}_{slot}";
                owner.AddHitArea(name, rect, null, () =>
                {
                    ctx.CustomData["SelectedDay"] = day;
                    ctx.CustomData["SelectedSlot"] = slot;
                    notifyAreaHit?.Invoke(name, rect);
                    Owner?.Invalidate();
                });
            }
            // Day headers
            foreach (var (rect, day) in _dayHeaderRects)
            {
                string name = $"Availability_Day_{day}";
                owner.AddHitArea(name, rect, null, () =>
                {
                    ctx.CustomData["HeaderDayClicked"] = day;
                    notifyAreaHit?.Invoke(name, rect);
                    Owner?.Invalidate();
                });
            }

            if (!_gridRectCache.IsEmpty)
            {
                owner.AddHitArea("Availability_Grid", _gridRectCache, null, () =>
                {
                    ctx.CustomData["GridClicked"] = true;
                    notifyAreaHit?.Invoke("Availability_Grid", _gridRectCache);
                    Owner?.Invalidate();
                });
            }
        }
    }
}