using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    internal class ListViewRenderer : ICalendarViewRenderer
    {
        public void Draw(Graphics g, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            var events = ctx.EventService.GetEventsForMonth(ctx.State.CurrentDate).OrderBy(e => e.StartTime).ToList();
            int yPos = grid.Y + 10;
            foreach (var evt in events)
            {
                var rect = new Rectangle(grid.X + 10, yPos, grid.Width - 20, 60);
                DrawListEvent(g, ctx, evt, rect);
                yPos += 70;
            }
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            var events = ctx.EventService.GetEventsForMonth(ctx.State.CurrentDate).OrderBy(e => e.StartTime).ToList();
            int yPos = grid.Y + 10;
            foreach (var evt in events)
            {
                var rect = new Rectangle(grid.X + 10, yPos, grid.Width - 20, 60);
                if (rect.Contains(location))
                {
                    ctx.State.SelectedEvent = evt;
                    return;
                }
                yPos += 70;
            }
        }

        private void DrawListEvent(Graphics g, CalendarRenderContext ctx, CalendarEvent evt, Rectangle rect)
        {
            var category = ctx.Categories.FirstOrDefault(c => c.Id == evt.CategoryId);
            var color = category?.Color ?? Color.Gray;

            using (var brush = new SolidBrush(Color.White))
                g.FillRectangle(brush, rect);

            using (var brush = new SolidBrush(color))
                g.FillRectangle(brush, new Rectangle(rect.X, rect.Y, 4, rect.Height));

            string eventText = $"{evt.Title}\n{evt.StartTime:MMM dd, yyyy HH:mm} - {evt.EndTime:HH:mm}" +
                               (string.IsNullOrEmpty(evt.Description) ? string.Empty : $"\n{evt.Description}");
            using (var brush = new SolidBrush(Color.Black))
                g.DrawString(eventText, ctx.EventFont, brush, new Rectangle(rect.X + 10, rect.Y + 5, rect.Width - 15, rect.Height - 10));

            using (var pen = new Pen(Color.FromArgb(218, 220, 224)))
                g.DrawRectangle(pen, rect);
        }
    }
}
