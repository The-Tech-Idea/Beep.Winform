using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(0,43,54);
            this.CheckBoxForeColor = Color.FromArgb(147,161,161);
            this.CheckBoxBorderColor = Color.FromArgb(88,110,117);
            this.CheckBoxCheckedBackColor = Color.FromArgb(0,43,54);
            this.CheckBoxCheckedForeColor = Color.FromArgb(147,161,161);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(88,110,117);
            this.CheckBoxHoverBackColor = Color.FromArgb(0,43,54);
            this.CheckBoxHoverForeColor = Color.FromArgb(147,161,161);
            this.CheckBoxHoverBorderColor = Color.FromArgb(88,110,117);
        }
    }
}