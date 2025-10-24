using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(40,40,40);
            this.CheckBoxForeColor = Color.FromArgb(235,219,178);
            this.CheckBoxBorderColor = Color.FromArgb(168,153,132);
            this.CheckBoxCheckedBackColor = Color.FromArgb(40,40,40);
            this.CheckBoxCheckedForeColor = Color.FromArgb(235,219,178);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(168,153,132);
            this.CheckBoxHoverBackColor = Color.FromArgb(40,40,40);
            this.CheckBoxHoverForeColor = Color.FromArgb(235,219,178);
            this.CheckBoxHoverBorderColor = Color.FromArgb(168,153,132);
        }
    }
}