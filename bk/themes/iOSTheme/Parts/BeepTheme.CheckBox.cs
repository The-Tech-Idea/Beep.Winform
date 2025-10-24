using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(242,242,247);
            this.CheckBoxForeColor = Color.FromArgb(28,28,30);
            this.CheckBoxBorderColor = Color.FromArgb(198,198,207);
            this.CheckBoxCheckedBackColor = Color.FromArgb(242,242,247);
            this.CheckBoxCheckedForeColor = Color.FromArgb(28,28,30);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(198,198,207);
            this.CheckBoxHoverBackColor = Color.FromArgb(242,242,247);
            this.CheckBoxHoverForeColor = Color.FromArgb(28,28,30);
            this.CheckBoxHoverBorderColor = Color.FromArgb(198,198,207);
        }
    }
}