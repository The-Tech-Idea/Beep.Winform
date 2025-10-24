using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(246,245,244);
            this.CheckBoxForeColor = Color.FromArgb(46,52,54);
            this.CheckBoxBorderColor = Color.FromArgb(205,207,212);
            this.CheckBoxCheckedBackColor = Color.FromArgb(246,245,244);
            this.CheckBoxCheckedForeColor = Color.FromArgb(46,52,54);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(205,207,212);
            this.CheckBoxHoverBackColor = Color.FromArgb(246,245,244);
            this.CheckBoxHoverForeColor = Color.FromArgb(46,52,54);
            this.CheckBoxHoverBorderColor = Color.FromArgb(205,207,212);
        }
    }
}