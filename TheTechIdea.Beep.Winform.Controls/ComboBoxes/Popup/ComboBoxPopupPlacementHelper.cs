using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal static class ComboBoxPopupPlacementHelper
    {
        public static (Point Location, int Height) Calculate(
            Control owner,
            int popupWidth,
            int preferredHeight,
            bool autoFlip = true)
        {
            Rectangle screenBounds = owner.RectangleToScreen(owner.ClientRectangle);
            Rectangle workingArea = Screen.FromControl(owner).WorkingArea;

            int spaceBelow = workingArea.Bottom - screenBounds.Bottom;
            int spaceAbove = screenBounds.Top - workingArea.Top;

            int actualHeight = preferredHeight;
            Point location;

            bool placeBelow = !autoFlip || spaceBelow >= preferredHeight || spaceBelow >= spaceAbove;
            if (placeBelow)
            {
                actualHeight = Math.Min(preferredHeight, Math.Max(0, spaceBelow - 4));
                location = new Point(screenBounds.Left, screenBounds.Bottom);
            }
            else
            {
                actualHeight = Math.Min(preferredHeight, Math.Max(0, spaceAbove - 4));
                location = new Point(screenBounds.Left, screenBounds.Top - actualHeight);
            }

            return (location, actualHeight);
        }
    }
}
