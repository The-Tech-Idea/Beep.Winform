using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(10,8,20);
            this.LabelForeColor = Color.FromArgb(228,244,255);
            this.LabelBorderColor = Color.FromArgb(90,20,110);
            this.LabelHoverBorderColor = Color.FromArgb(90,20,110);
            this.LabelHoverBackColor = Color.FromArgb(10,8,20);
            this.LabelHoverForeColor = Color.FromArgb(228,244,255);
            this.LabelSelectedBorderColor = Color.FromArgb(90,20,110);
            this.LabelSelectedBackColor = Color.FromArgb(10,8,20);
            this.LabelSelectedForeColor = Color.FromArgb(228,244,255);
            this.LabelDisabledBackColor = Color.FromArgb(10,8,20);
            this.LabelDisabledForeColor = Color.FromArgb(228,244,255);
            this.LabelDisabledBorderColor = Color.FromArgb(90,20,110);
        }
    }
}