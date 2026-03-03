using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Sheets
{
    internal static class BottomSheetLayoutHelper
    {
        public static Rectangle GetGripRect(Rectangle bounds) => new Rectangle(bounds.X + (bounds.Width / 2) - 24, bounds.Y + 8, 48, 6);
    }
}
