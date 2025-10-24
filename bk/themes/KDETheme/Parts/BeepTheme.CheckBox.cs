using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(248,249,250);
            this.CheckBoxForeColor = Color.FromArgb(33,37,41);
            this.CheckBoxBorderColor = Color.FromArgb(222,226,230);
            this.CheckBoxCheckedBackColor = Color.FromArgb(248,249,250);
            this.CheckBoxCheckedForeColor = Color.FromArgb(33,37,41);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(222,226,230);
            this.CheckBoxHoverBackColor = Color.FromArgb(248,249,250);
            this.CheckBoxHoverForeColor = Color.FromArgb(33,37,41);
            this.CheckBoxHoverBorderColor = Color.FromArgb(222,226,230);
        }
    }
}