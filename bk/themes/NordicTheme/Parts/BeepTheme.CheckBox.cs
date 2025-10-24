using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(250,250,251);
            this.CheckBoxForeColor = Color.FromArgb(31,41,55);
            this.CheckBoxBorderColor = Color.FromArgb(229,231,235);
            this.CheckBoxCheckedBackColor = Color.FromArgb(250,250,251);
            this.CheckBoxCheckedForeColor = Color.FromArgb(31,41,55);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(229,231,235);
            this.CheckBoxHoverBackColor = Color.FromArgb(250,250,251);
            this.CheckBoxHoverForeColor = Color.FromArgb(31,41,55);
            this.CheckBoxHoverBorderColor = Color.FromArgb(229,231,235);
        }
    }
}