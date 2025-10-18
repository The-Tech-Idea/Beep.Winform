using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(250,250,250);
            this.CheckBoxForeColor = Color.FromArgb(33,33,33);
            this.CheckBoxBorderColor = Color.FromArgb(224,224,224);
            this.CheckBoxCheckedBackColor = Color.FromArgb(250,250,250);
            this.CheckBoxCheckedForeColor = Color.FromArgb(33,33,33);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(224,224,224);
            this.CheckBoxHoverBackColor = Color.FromArgb(250,250,250);
            this.CheckBoxHoverForeColor = Color.FromArgb(33,33,33);
            this.CheckBoxHoverBorderColor = Color.FromArgb(224,224,224);
        }
    }
}