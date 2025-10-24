using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(255,255,255);
            this.CheckBoxForeColor = Color.FromArgb(17,24,39);
            this.CheckBoxBorderColor = Color.FromArgb(203,213,225);
            this.CheckBoxCheckedBackColor = Color.FromArgb(255,255,255);
            this.CheckBoxCheckedForeColor = Color.FromArgb(17,24,39);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(203,213,225);
            this.CheckBoxHoverBackColor = Color.FromArgb(255,255,255);
            this.CheckBoxHoverForeColor = Color.FromArgb(17,24,39);
            this.CheckBoxHoverBorderColor = Color.FromArgb(203,213,225);
        }
    }
}