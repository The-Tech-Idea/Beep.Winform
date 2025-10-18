using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(10,12,20);
            this.CheckBoxForeColor = Color.FromArgb(235,245,255);
            this.CheckBoxBorderColor = Color.FromArgb(60,70,100);
            this.CheckBoxCheckedBackColor = Color.FromArgb(10,12,20);
            this.CheckBoxCheckedForeColor = Color.FromArgb(235,245,255);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(60,70,100);
            this.CheckBoxHoverBackColor = Color.FromArgb(10,12,20);
            this.CheckBoxHoverForeColor = Color.FromArgb(235,245,255);
            this.CheckBoxHoverBorderColor = Color.FromArgb(60,70,100);
        }
    }
}