using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(245,246,248);
            this.CheckBoxForeColor = Color.FromArgb(32,32,32);
            this.CheckBoxBorderColor = Color.FromArgb(218,223,230);
            this.CheckBoxCheckedBackColor = Color.FromArgb(245,246,248);
            this.CheckBoxCheckedForeColor = Color.FromArgb(32,32,32);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(218,223,230);
            this.CheckBoxHoverBackColor = Color.FromArgb(245,246,248);
            this.CheckBoxHoverForeColor = Color.FromArgb(32,32,32);
            this.CheckBoxHoverBorderColor = Color.FromArgb(218,223,230);
        }
    }
}