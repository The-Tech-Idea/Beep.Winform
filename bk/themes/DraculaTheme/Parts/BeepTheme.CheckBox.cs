using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(40,42,54);
            this.CheckBoxForeColor = Color.FromArgb(248,248,242);
            this.CheckBoxBorderColor = Color.FromArgb(98,114,164);
            this.CheckBoxCheckedBackColor = Color.FromArgb(40,42,54);
            this.CheckBoxCheckedForeColor = Color.FromArgb(248,248,242);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(98,114,164);
            this.CheckBoxHoverBackColor = Color.FromArgb(40,42,54);
            this.CheckBoxHoverForeColor = Color.FromArgb(248,248,242);
            this.CheckBoxHoverBorderColor = Color.FromArgb(98,114,164);
        }
    }
}