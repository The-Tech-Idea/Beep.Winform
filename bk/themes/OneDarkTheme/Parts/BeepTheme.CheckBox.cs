using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(40,44,52);
            this.CheckBoxForeColor = Color.FromArgb(171,178,191);
            this.CheckBoxBorderColor = Color.FromArgb(92,99,112);
            this.CheckBoxCheckedBackColor = Color.FromArgb(40,44,52);
            this.CheckBoxCheckedForeColor = Color.FromArgb(171,178,191);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(92,99,112);
            this.CheckBoxHoverBackColor = Color.FromArgb(40,44,52);
            this.CheckBoxHoverForeColor = Color.FromArgb(171,178,191);
            this.CheckBoxHoverBorderColor = Color.FromArgb(92,99,112);
        }
    }
}