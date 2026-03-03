using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Sheets.Painters
{
    internal static class BottomSheetPainter
    {
        public static void PaintGrip(Graphics g, Rectangle bounds)
        {
            var grip = BottomSheetLayoutHelper.GetGripRect(bounds);
            using var path = new GraphicsPath();
            path.AddArc(grip.X, grip.Y, grip.Height, grip.Height, 90, 180);
            path.AddArc(grip.Right - grip.Height, grip.Y, grip.Height, grip.Height, 270, 180);
            path.CloseFigure();
            using var brush = new SolidBrush(Color.FromArgb(120, Color.Gray));
            g.FillPath(brush, path);
        }
    }
}
