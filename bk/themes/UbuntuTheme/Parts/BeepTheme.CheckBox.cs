using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(242,242,245);
            this.CheckBoxForeColor = Color.FromArgb(44,44,44);
            this.CheckBoxBorderColor = Color.FromArgb(218,218,222);
            this.CheckBoxCheckedBackColor = Color.FromArgb(242,242,245);
            this.CheckBoxCheckedForeColor = Color.FromArgb(44,44,44);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(218,218,222);
            this.CheckBoxHoverBackColor = Color.FromArgb(242,242,245);
            this.CheckBoxHoverForeColor = Color.FromArgb(44,44,44);
            this.CheckBoxHoverBorderColor = Color.FromArgb(218,218,222);
        }
    }
}