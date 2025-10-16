using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(255,255,255);
            this.LabelForeColor = Color.FromArgb(17,24,39);
            this.LabelBorderColor = Color.FromArgb(203,213,225);
            this.LabelHoverBorderColor = Color.FromArgb(203,213,225);
            this.LabelHoverBackColor = Color.FromArgb(255,255,255);
            this.LabelHoverForeColor = Color.FromArgb(17,24,39);
            this.LabelSelectedBorderColor = Color.FromArgb(203,213,225);
            this.LabelSelectedBackColor = Color.FromArgb(255,255,255);
            this.LabelSelectedForeColor = Color.FromArgb(17,24,39);
            this.LabelDisabledBackColor = Color.FromArgb(255,255,255);
            this.LabelDisabledForeColor = Color.FromArgb(17,24,39);
            this.LabelDisabledBorderColor = Color.FromArgb(203,213,225);
        }
    }
}