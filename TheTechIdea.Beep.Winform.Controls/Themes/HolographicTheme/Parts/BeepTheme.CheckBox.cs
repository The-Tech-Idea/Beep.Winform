using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(15,16,32);
            this.CheckBoxForeColor = Color.FromArgb(245,247,255);
            this.CheckBoxBorderColor = Color.FromArgb(74,79,123);
            this.CheckBoxCheckedBackColor = Color.FromArgb(15,16,32);
            this.CheckBoxCheckedForeColor = Color.FromArgb(245,247,255);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(74,79,123);
            this.CheckBoxHoverBackColor = Color.FromArgb(15,16,32);
            this.CheckBoxHoverForeColor = Color.FromArgb(245,247,255);
            this.CheckBoxHoverBorderColor = Color.FromArgb(74,79,123);
        }
    }
}