using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;
using TheTechIdea.Beep.Winform.Controls.Switchs.Helpers;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    // NOTE: SwitchOrientation enum moved to Switchs/Models/SwitchOrientation.cs

    [ToolboxItem(true)]
    [DisplayName("Beep Switch")]
    [Category("Beep Controls")]
    [Description("A cylindrical toggle switch control with customizable labels, images, and orientation.")]
    public partial class BeepSwitch : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.SwitchStandard;
        protected internal override Padding StylePadding => new Padding(0);
        // NOTE: Fields moved to BeepSwitch.Core.cs
        // NOTE: Events and Properties moved to BeepSwitch.Properties.cs

        // NOTE: Constructor moved to BeepSwitch.Core.cs

        // NOTE: Painting moved to BeepSwitch.Drawing.cs
        // NOTE: Legacy drawing methods below are kept for reference but not used
        




        /// <summary>
        /// LEGACY: Creates a capsule-shaped GraphicsPath (replaced by CreateTrackPath in Drawing.cs).
        /// </summary>
        /// <param name="rect">The rectangle for the capsule.</param>
        /// <param name="vertical">If true, creates a vertical capsule; otherwise horizontal.</param>
        private GraphicsPath GetCapsulePath_Legacy(Rectangle rect, bool vertical)
        {
            GraphicsPath path = new GraphicsPath();
            if (vertical)
            {
                int radius = rect.Width / 2;
                // Top arc.
                path.AddArc(rect.X, rect.Y, rect.Width, 2 * radius, 180, 180);
                // Bottom arc.
                path.AddArc(rect.X, rect.Bottom - 2 * radius, rect.Width, 2 * radius, 0, 180);
            }
            else
            {
                int radius = rect.Height / 2;
                // Left arc.
                path.AddArc(rect.X, rect.Y, rect.Height, rect.Height, 90, 180);
                // Right arc.
                path.AddArc(rect.Right - rect.Height, rect.Y, rect.Height, rect.Height, 270, 180);
            }
            path.CloseFigure();
            return path;
        }

        // NOTE: Mouse Interaction moved to BeepSwitch.Interaction.cs

        // NOTE: Event Raisers moved to BeepSwitch.Properties.cs
        // NOTE: Data Binding Methods moved to BeepSwitch.DataBinding.cs

        // NOTE: Theme Support moved to BeepSwitch.Theme.cs

        #region Disposal

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _onBeepImage?.Dispose();
                _offBeepImage?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
