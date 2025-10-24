using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(236,244,255);
            this.LabelForeColor = Color.FromArgb(17,24,39);
            this.LabelBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.LabelHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.LabelHoverBackColor = Color.FromArgb(236,244,255);
            this.LabelHoverForeColor = Color.FromArgb(17,24,39);
            this.LabelSelectedBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.LabelSelectedBackColor = Color.FromArgb(236,244,255);
            this.LabelSelectedForeColor = Color.FromArgb(17,24,39);
            this.LabelDisabledBackColor = Color.FromArgb(236,244,255);
            this.LabelDisabledForeColor = Color.FromArgb(17,24,39);
            this.LabelDisabledBorderColor = Color.FromArgb(140, 255, 255, 255);
        }
    }
}