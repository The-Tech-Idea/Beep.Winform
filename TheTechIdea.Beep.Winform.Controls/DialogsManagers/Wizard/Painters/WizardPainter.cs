using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Wizard.Painters
{
    internal static class WizardPainter
    {
        public static void PaintStepDots(Graphics g, Rectangle bounds, int total, int active)
        {
            if (total <= 0) return;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int size = 10;
            int spacing = 12;
            int width = (total * size) + ((total - 1) * spacing);
            int startX = bounds.X + (bounds.Width - width) / 2;
            int y = bounds.Bottom - 16;
            for (int i = 0; i < total; i++)
            {
                var dot = new Rectangle(startX + i * (size + spacing), y, size, size);
                using var brush = new SolidBrush(i == active ? Color.FromArgb(59, 130, 246) : Color.FromArgb(190, 190, 190));
                g.FillEllipse(brush, dot);
            }
        }
    }
}
