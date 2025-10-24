using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(245,246,247);
            this.CheckBoxForeColor = Color.FromArgb(43,45,48);
            this.CheckBoxBorderColor = Color.FromArgb(220,223,230);
            this.CheckBoxCheckedBackColor = Color.FromArgb(245,246,247);
            this.CheckBoxCheckedForeColor = Color.FromArgb(43,45,48);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(220,223,230);
            this.CheckBoxHoverBackColor = Color.FromArgb(245,246,247);
            this.CheckBoxHoverForeColor = Color.FromArgb(43,45,48);
            this.CheckBoxHoverBorderColor = Color.FromArgb(220,223,230);
        }
    }
}