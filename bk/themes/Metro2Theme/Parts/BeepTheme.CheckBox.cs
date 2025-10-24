using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(243,242,241);
            this.CheckBoxForeColor = Color.FromArgb(32,31,30);
            this.CheckBoxBorderColor = Color.FromArgb(220,220,220);
            this.CheckBoxCheckedBackColor = Color.FromArgb(243,242,241);
            this.CheckBoxCheckedForeColor = Color.FromArgb(32,31,30);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(220,220,220);
            this.CheckBoxHoverBackColor = Color.FromArgb(243,242,241);
            this.CheckBoxHoverForeColor = Color.FromArgb(32,31,30);
            this.CheckBoxHoverBorderColor = Color.FromArgb(220,220,220);
        }
    }
}