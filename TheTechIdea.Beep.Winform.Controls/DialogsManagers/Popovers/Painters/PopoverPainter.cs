using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Popovers.Painters
{
    internal static class PopoverPainter
    {
        public static void PaintBorder(Graphics g, Rectangle bounds)
        {
            using var pen = new Pen(Color.FromArgb(180, 180, 180));
            g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
        }
    }
}
