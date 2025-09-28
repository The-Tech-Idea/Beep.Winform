using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// TimeSlots - Available time slot picker painter
    /// </summary>
    internal sealed class TimeSlotsPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            // Title header
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                24
            );

            // Time slots grid
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - ctx.HeaderRect.Bottom - pad * 2
            );

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 6, layers: 2, offset: 1);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var timeSlots = ctx.CustomData.ContainsKey("TimeSlots") ?
                (List<TimeSlot>)ctx.CustomData["TimeSlots"] : new List<TimeSlot>();
            var selectedDate = ctx.CustomData.ContainsKey("SelectedDate") ?
                (DateTime)ctx.CustomData["SelectedDate"] : DateTime.Today;

            // Draw header
            using var headerFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var headerBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            string headerText = $"Available Times - {selectedDate:MMM dd, yyyy}";
            g.DrawString(headerText, headerFont, headerBrush, ctx.HeaderRect);

            // Draw time slots
            DrawTimeSlots(g, ctx.ContentRect, timeSlots, ctx.AccentColor);
        }

        private void DrawTimeSlots(Graphics g, Rectangle rect, List<TimeSlot> timeSlots, Color accentColor)
        {
            if (timeSlots.Count == 0) return;

            int columns = 2;
            int rows = (int)Math.Ceiling(timeSlots.Count / (double)columns);

            float cellWidth = rect.Width / (float)columns;
            float cellHeight = Math.Min(40f, rect.Height / (float)Math.Max(rows, 1));

            using var timeFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
            using var availableBrush = new SolidBrush(Color.FromArgb(100, 200, 100)); // Light green
            using var bookedBrush = new SolidBrush(Color.FromArgb(200, 100, 100)); // Light red
            using var textBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
            using var borderPen = new Pen(Color.FromArgb(150, Color.Black), 1);

            for (int i = 0; i < timeSlots.Count && i < columns * rows; i++)
            {
                var slot = timeSlots[i];
                int row = i / columns;
                int col = i % columns;

                var slotRect = new RectangleF(
                    rect.X + col * cellWidth + 4,
                    rect.Y + row * cellHeight + 4,
                    cellWidth - 8,
                    cellHeight - 8
                );

                // Draw slot background
                var bgBrush = slot.IsAvailable ? availableBrush : bookedBrush;
                using var slotPath = CreateRoundedPath(Rectangle.Round(slotRect), 8);
                g.FillPath(bgBrush, slotPath);
                g.DrawPath(borderPen, slotPath);

                // Draw time text
                var textFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(slot.Label, timeFont, textBrush, slotRect, textFormat);

                // Draw status indicator
                DrawSlotStatus(g, slotRect, slot, accentColor);
            }
        }

        private void DrawSlotStatus(Graphics g, RectangleF slotRect, TimeSlot slot, Color accentColor)
        {
            // Draw small status indicator in corner
            float indicatorSize = 8f;
            var indicatorRect = new RectangleF(
                slotRect.Right - indicatorSize - 4,
                slotRect.Top + 4,
                indicatorSize,
                indicatorSize
            );

            Color statusColor = slot.IsAvailable ? Color.Green :
                               slot.IsBooked ? Color.Red : Color.Gray;

            using var statusBrush = new SolidBrush(statusColor);
            g.FillEllipse(statusBrush, indicatorRect);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw time slot legends
        }
    }
}