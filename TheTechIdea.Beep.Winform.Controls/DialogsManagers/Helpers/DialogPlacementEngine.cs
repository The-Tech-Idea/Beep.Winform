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

            return ClampToWorkingArea(desired, size, work);
        }

        /// <summary>
        /// Places a modeless window at a corner of, or centred on, its owner.
        /// </summary>
        /// <remarks>
        /// <see cref="DialogPosition"/> anchors to corners while <see cref="DialogPlacementStrategy"/>
        /// chooses a strategy, so the two enums are not interchangeable — but their centring cases and
        /// their on-screen clamping are the same problem, and only one of them solved it. The version
        /// in BeepModelessDialog centred on the owner for <see cref="DialogPosition.CenterScreen"/>,
        /// because that value had no case and fell through to the default, and it clamped to nothing:
        /// a corner of an owner near the screen edge put the window off-screen.
        /// </remarks>
        public static Point Place(Form owner, Size size, DialogPosition position, int cornerOffset)
        {
            if (owner == null)
            {
                var wa = Screen.PrimaryScreen.WorkingArea;
                return new Point(wa.Left + (wa.Width - size.Width) / 2, wa.Top + (wa.Height - size.Height) / 2);
            }

            var work = Screen.FromControl(owner).WorkingArea;
            var desired = position switch
            {
                DialogPosition.TopLeft => new Point(owner.Left + cornerOffset, owner.Top + cornerOffset),
                DialogPosition.TopRight => new Point(owner.Right - size.Width - cornerOffset, owner.Top + cornerOffset),
                DialogPosition.BottomLeft => new Point(owner.Left + cornerOffset, owner.Bottom - size.Height - cornerOffset),
                DialogPosition.BottomRight => new Point(owner.Right - size.Width - cornerOffset, owner.Bottom - size.Height - cornerOffset),
                DialogPosition.CenterScreen => new Point(work.Left + (work.Width - size.Width) / 2,
                                                        work.Top + (work.Height - size.Height) / 2),
                _ => new Point(owner.Left + (owner.Width - size.Width) / 2,
                               owner.Top + (owner.Height - size.Height) / 2)
            };

            return ClampToWorkingArea(desired, size, work);
        }

        /// <summary>Keeps a window on screen; shared so both placement paths clamp identically.</summary>
        private static Point ClampToWorkingArea(Point desired, Size size, Rectangle work)
        {
            if (desired.X + size.Width > work.Right) desired.X = work.Right - size.Width - 8;
            if (desired.Y + size.Height > work.Bottom) desired.Y = work.Bottom - size.Height - 8;
            if (desired.X < work.Left) desired.X = work.Left + 8;
            if (desired.Y < work.Top) desired.Y = work.Top + 8;
            return desired;
        }
    }
}
