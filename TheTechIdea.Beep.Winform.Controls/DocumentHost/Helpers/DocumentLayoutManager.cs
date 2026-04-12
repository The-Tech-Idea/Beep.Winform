using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Helpers
{
    public class DocumentLayoutManager
    {
        public Rectangle CalculateTabStripBounds(Control owner, TabStripPosition position, int tabHeight)
        {
            int scaledHeight = DpiScalingHelper.ScaleValue(tabHeight, owner);
            switch (position)
            {
                case TabStripPosition.Top:
                    return new Rectangle(0, 0, owner.Width, scaledHeight);
                case TabStripPosition.Bottom:
                    return new Rectangle(0, owner.Height - scaledHeight, owner.Width, scaledHeight);
                case TabStripPosition.Left:
                    return new Rectangle(0, 0, scaledHeight, owner.Height);
                case TabStripPosition.Right:
                    return new Rectangle(owner.Width - scaledHeight, 0, scaledHeight, owner.Height);
                default:
                    return Rectangle.Empty;
            }
        }

        public Rectangle CalculateContentAreaBounds(Control owner, TabStripPosition position, int tabHeight)
        {
            int scaledHeight = DpiScalingHelper.ScaleValue(tabHeight, owner);
            switch (position)
            {
                case TabStripPosition.Top:
                    return new Rectangle(0, scaledHeight, owner.Width, owner.Height - scaledHeight);
                case TabStripPosition.Bottom:
                    return new Rectangle(0, 0, owner.Width, owner.Height - scaledHeight);
                case TabStripPosition.Left:
                    return new Rectangle(scaledHeight, 0, owner.Width - scaledHeight, owner.Height);
                case TabStripPosition.Right:
                    return new Rectangle(0, 0, owner.Width - scaledHeight, owner.Height);
                default:
                    return owner.ClientRectangle;
            }
        }

        public bool IsVertical(TabStripPosition position)
        {
            return position == TabStripPosition.Left || position == TabStripPosition.Right;
        }

        public bool IsVisible(TabStripPosition position)
        {
            return position != TabStripPosition.Hidden;
        }
    }
}
