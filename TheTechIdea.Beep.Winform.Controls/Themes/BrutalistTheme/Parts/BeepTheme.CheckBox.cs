using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(250,250,250);
            this.CheckBoxForeColor = Color.FromArgb(20,20,20);
            this.CheckBoxBorderColor = Color.FromArgb(0,0,0);
            this.CheckBoxCheckedBackColor = Color.FromArgb(250,250,250);
            this.CheckBoxCheckedForeColor = Color.FromArgb(20,20,20);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(0,0,0);
            this.CheckBoxHoverBackColor = Color.FromArgb(250,250,250);
            this.CheckBoxHoverForeColor = Color.FromArgb(20,20,20);
            this.CheckBoxHoverBorderColor = Color.FromArgb(0,0,0);
        }
    }
}