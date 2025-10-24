using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(236,244,255);
            this.CheckBoxForeColor = Color.FromArgb(17,24,39);
            this.CheckBoxBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.CheckBoxCheckedBackColor = Color.FromArgb(236,244,255);
            this.CheckBoxCheckedForeColor = Color.FromArgb(17,24,39);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.CheckBoxHoverBackColor = Color.FromArgb(236,244,255);
            this.CheckBoxHoverForeColor = Color.FromArgb(17,24,39);
            this.CheckBoxHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
        }
    }
}