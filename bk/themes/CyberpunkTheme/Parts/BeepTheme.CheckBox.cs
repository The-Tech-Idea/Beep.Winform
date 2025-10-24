using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(10,8,20);
            this.CheckBoxForeColor = Color.FromArgb(228,244,255);
            this.CheckBoxBorderColor = Color.FromArgb(90,20,110);
            this.CheckBoxCheckedBackColor = Color.FromArgb(10,8,20);
            this.CheckBoxCheckedForeColor = Color.FromArgb(228,244,255);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(90,20,110);
            this.CheckBoxHoverBackColor = Color.FromArgb(10,8,20);
            this.CheckBoxHoverForeColor = Color.FromArgb(228,244,255);
            this.CheckBoxHoverBorderColor = Color.FromArgb(90,20,110);
        }
    }
}