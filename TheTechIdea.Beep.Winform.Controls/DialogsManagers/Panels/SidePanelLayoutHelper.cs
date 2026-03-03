using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Panels
{
    internal static class SidePanelLayoutHelper
    {
        public static Rectangle GetHeaderRect(Rectangle bounds) => new Rectangle(bounds.X, bounds.Y, bounds.Width, 48);
    }
}
