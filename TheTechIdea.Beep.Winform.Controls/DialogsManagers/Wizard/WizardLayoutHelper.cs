using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Wizard
{
    internal static class WizardLayoutHelper
    {
        public static Rectangle GetHeaderRect(Rectangle bounds) => new Rectangle(bounds.X + 12, bounds.Y + 12, bounds.Width - 24, 42);
        public static Rectangle GetContentRect(Rectangle bounds) => new Rectangle(bounds.X + 12, bounds.Y + 60, bounds.Width - 24, bounds.Height - 130);
        public static Rectangle GetButtonsRect(Rectangle bounds) => new Rectangle(bounds.X + 12, bounds.Bottom - 56, bounds.Width - 24, 44);
    }
}
