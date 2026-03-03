using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Panels.Painters
{
    internal static class SidePanelPainter
    {
        public static void PaintHeader(Graphics g, Rectangle bounds, string title)
        {
            var header = SidePanelLayoutHelper.GetHeaderRect(bounds);
            using var brush = new SolidBrush(Color.FromArgb(245, 245, 245));
            g.FillRectangle(brush, header);
            using var textBrush = new SolidBrush(Color.FromArgb(50, 50, 50));
            using var font = new Font("Segoe UI", 10, FontStyle.Bold);
            g.DrawString(title, font, textBrush, new RectangleF(header.X + 12, header.Y + 12, header.Width - 24, 22));
        }
    }
}
