using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(255,251,235);
            this.CheckBoxForeColor = Color.FromArgb(33,37,41);
            this.CheckBoxBorderColor = Color.FromArgb(247,208,136);
            this.CheckBoxCheckedBackColor = Color.FromArgb(255,251,235);
            this.CheckBoxCheckedForeColor = Color.FromArgb(33,37,41);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(247,208,136);
            this.CheckBoxHoverBackColor = Color.FromArgb(255,251,235);
            this.CheckBoxHoverForeColor = Color.FromArgb(33,37,41);
            this.CheckBoxHoverBorderColor = Color.FromArgb(247,208,136);
        }
    }
}