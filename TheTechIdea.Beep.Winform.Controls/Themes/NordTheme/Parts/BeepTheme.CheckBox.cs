using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(46,52,64);
            this.CheckBoxForeColor = Color.FromArgb(216,222,233);
            this.CheckBoxBorderColor = Color.FromArgb(76,86,106);
            this.CheckBoxCheckedBackColor = Color.FromArgb(46,52,64);
            this.CheckBoxCheckedForeColor = Color.FromArgb(216,222,233);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(76,86,106);
            this.CheckBoxHoverBackColor = Color.FromArgb(46,52,64);
            this.CheckBoxHoverForeColor = Color.FromArgb(216,222,233);
            this.CheckBoxHoverBorderColor = Color.FromArgb(76,86,106);
        }
    }
}