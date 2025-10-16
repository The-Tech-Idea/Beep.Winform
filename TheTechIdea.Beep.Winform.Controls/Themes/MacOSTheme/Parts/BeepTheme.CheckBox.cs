using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(250,250,252);
            this.CheckBoxForeColor = Color.FromArgb(28,28,30);
            this.CheckBoxBorderColor = Color.FromArgb(229,229,234);
            this.CheckBoxCheckedBackColor = Color.FromArgb(250,250,252);
            this.CheckBoxCheckedForeColor = Color.FromArgb(28,28,30);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(229,229,234);
            this.CheckBoxHoverBackColor = Color.FromArgb(250,250,252);
            this.CheckBoxHoverForeColor = Color.FromArgb(28,28,30);
            this.CheckBoxHoverBorderColor = Color.FromArgb(229,229,234);
        }
    }
}