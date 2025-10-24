using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(26,27,38);
            this.CheckBoxForeColor = Color.FromArgb(192,202,245);
            this.CheckBoxBorderColor = Color.FromArgb(86,95,137);
            this.CheckBoxCheckedBackColor = Color.FromArgb(26,27,38);
            this.CheckBoxCheckedForeColor = Color.FromArgb(192,202,245);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(86,95,137);
            this.CheckBoxHoverBackColor = Color.FromArgb(26,27,38);
            this.CheckBoxHoverForeColor = Color.FromArgb(192,202,245);
            this.CheckBoxHoverBorderColor = Color.FromArgb(86,95,137);
        }
    }
}