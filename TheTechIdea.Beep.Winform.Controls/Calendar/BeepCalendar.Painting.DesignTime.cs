using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// Simple placeholder rendering for Visual Studio Designer
        /// </summary>
        private void PaintDesignTimePlaceholder(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(248, 249, 250)), ClientRectangle);
            using (var pen = new Pen(Color.FromArgb(218, 220, 224), 1))
            {
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
            using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
            {
                g.DrawString("BeepCalendar (Design Mode)", font, brush, 10, 10);
            }
            using (var font = new Font("Segoe UI", 10))
            using (var brush = new SolidBrush(Color.FromArgb(120, 120, 120)))
            {
                g.DrawString($"View: {_state.ViewMode}", font, brush, 10, 35);
                g.DrawString($"Date: {_state.CurrentDate:MMMM yyyy}", font, brush, 10, 55);
                g.DrawString($"Events: {_events?.Count ?? 0}", font, brush, 10, 75);
            }

            // Draw a simple calendar grid preview
            int previewTop = 100;
            int cellWidth = Math.Min(40, (Width - 40) / 7);
            int cellHeight = 30;
            string[] days = { "S", "M", "T", "W", "T", "F", "S" };

            using (var font = new Font("Segoe UI", 9))
            using (var brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
            using (var linePen = new Pen(Color.FromArgb(200, 200, 200)))
            {
                for (int i = 0; i < 7 && (20 + i * cellWidth + cellWidth) < Width; i++)
                {
                    var rect = new Rectangle(20 + i * cellWidth, previewTop, cellWidth, cellHeight);
                    g.DrawRectangle(linePen, rect);
                    g.DrawString(days[i], font, brush, rect,
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }
            }
        }
    }
}