using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Popovers
{
    internal static class PopoverLayoutHelper
    {
        public static Point GetSmartLocation(Control anchor, Size size)
        {
            var screen = Screen.FromControl(anchor).WorkingArea;
            var p = anchor.PointToScreen(Point.Empty);
            int x = p.X;
            int y = p.Y + anchor.Height + 8;
            if (x + size.Width > screen.Right) x = screen.Right - size.Width - 8;
            if (y + size.Height > screen.Bottom) y = p.Y - size.Height - 8;
            if (x < screen.Left) x = screen.Left + 8;
            if (y < screen.Top) y = screen.Top + 8;
            return new Point(x, y);
        }
    }
}
