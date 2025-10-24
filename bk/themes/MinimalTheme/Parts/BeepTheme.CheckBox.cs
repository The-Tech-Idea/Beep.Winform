using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(255,255,255);
            this.CheckBoxForeColor = Color.FromArgb(31,41,55);
            this.CheckBoxBorderColor = Color.FromArgb(209,213,219);
            this.CheckBoxCheckedBackColor = Color.FromArgb(255,255,255);
            this.CheckBoxCheckedForeColor = Color.FromArgb(31,41,55);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(209,213,219);
            this.CheckBoxHoverBackColor = Color.FromArgb(255,255,255);
            this.CheckBoxHoverForeColor = Color.FromArgb(31,41,55);
            this.CheckBoxHoverBorderColor = Color.FromArgb(209,213,219);
        }
    }
}