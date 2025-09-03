using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls
{
    internal static class CaptionGlyphProvider
    {
        public static void DrawMinimize(Graphics g, Pen p, Rectangle bounds, float scale)
        {
            int y = bounds.Y + (int)(bounds.Height * 0.6f);
            int left = bounds.Left + (int)(6 * scale);
            int right = bounds.Right - (int)(6 * scale);
            g.DrawLine(p, left, y, right, y);
        }

        public static void DrawMaximize(Graphics g, Pen p, Rectangle bounds, float scale)
        {
            int w = (int)Math.Max(8 * scale, bounds.Width / 3f);
            int cx = bounds.X + bounds.Width / 2; int cy = bounds.Y + bounds.Height / 2;
            var rect = new Rectangle(cx - w / 2, cy - w / 2, w, w);
            g.DrawRectangle(p, rect);
        }

        public static void DrawRestore(Graphics g, Pen p, Rectangle bounds, float scale)
        {
            int w = (int)Math.Max(8 * scale, bounds.Width / 3f) - (int)(2 * scale);
            int cx = bounds.X + bounds.Width / 2; int cy = bounds.Y + bounds.Height / 2;
            g.DrawRectangle(p, new Rectangle(cx - w / 2 + (int)(2*scale), cy - w / 2 + (int)(2*scale), w, w));
            g.DrawRectangle(p, new Rectangle(cx - w / 2 - (int)(2*scale), cy - w / 2 - (int)(2*scale), w, w));
        }

        public static void DrawClose(Graphics g, Pen p, Rectangle bounds, float scale)
        {
            int inset = (int)(6 * scale);
            g.DrawLine(p, bounds.Left + inset, bounds.Top + inset, bounds.Right - inset, bounds.Bottom - inset);
            g.DrawLine(p, bounds.Right - inset, bounds.Top + inset, bounds.Left + inset, bounds.Bottom - inset);
        }
    }
}
