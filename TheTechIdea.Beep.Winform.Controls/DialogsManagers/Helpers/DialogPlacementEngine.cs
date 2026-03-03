using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    internal static class DialogPlacementEngine
    {
        public static Point Place(Form owner, Size size, DialogPlacementStrategy strategy)
        {
            if (owner == null)
            {
                var wa = Screen.PrimaryScreen.WorkingArea;
                return new Point(wa.Left + (wa.Width - size.Width) / 2, wa.Top + (wa.Height - size.Height) / 2);
            }

            var work = Screen.FromControl(owner).WorkingArea;
            var desired = strategy switch
            {
                DialogPlacementStrategy.CenterScreen => new Point(work.Left + (work.Width - size.Width) / 2, work.Top + (work.Height - size.Height) / 2),
                DialogPlacementStrategy.SmartNearest => new Point(owner.Left + 24, owner.Top + 24),
                _ => new Point(owner.Left + (owner.Width - size.Width) / 2, owner.Top + (owner.Height - size.Height) / 2)
            };

            if (desired.X + size.Width > work.Right) desired.X = work.Right - size.Width - 8;
            if (desired.Y + size.Height > work.Bottom) desired.Y = work.Bottom - size.Height - 8;
            if (desired.X < work.Left) desired.X = work.Left + 8;
            if (desired.Y < work.Top) desired.Y = work.Top + 8;
            return desired;
        }
    }
}
