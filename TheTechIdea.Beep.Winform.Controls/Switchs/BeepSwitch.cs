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
