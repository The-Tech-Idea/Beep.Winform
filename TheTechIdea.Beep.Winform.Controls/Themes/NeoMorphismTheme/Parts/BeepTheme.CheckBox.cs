using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(236,240,243);
            this.CheckBoxForeColor = Color.FromArgb(58,66,86);
            this.CheckBoxBorderColor = Color.FromArgb(221,228,235);
            this.CheckBoxCheckedBackColor = Color.FromArgb(236,240,243);
            this.CheckBoxCheckedForeColor = Color.FromArgb(58,66,86);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(221,228,235);
            this.CheckBoxHoverBackColor = Color.FromArgb(236,240,243);
            this.CheckBoxHoverForeColor = Color.FromArgb(58,66,86);
            this.CheckBoxHoverBorderColor = Color.FromArgb(221,228,235);
        }
    }
}