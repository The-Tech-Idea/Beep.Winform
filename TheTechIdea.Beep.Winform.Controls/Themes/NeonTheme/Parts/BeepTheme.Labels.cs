using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(10,12,20);
            this.LabelForeColor = Color.FromArgb(235,245,255);
            this.LabelBorderColor = Color.FromArgb(60,70,100);
            this.LabelHoverBorderColor = Color.FromArgb(60,70,100);
            this.LabelHoverBackColor = Color.FromArgb(10,12,20);
            this.LabelHoverForeColor = Color.FromArgb(235,245,255);
            this.LabelSelectedBorderColor = Color.FromArgb(60,70,100);
            this.LabelSelectedBackColor = Color.FromArgb(10,12,20);
            this.LabelSelectedForeColor = Color.FromArgb(235,245,255);
            this.LabelDisabledBackColor = Color.FromArgb(10,12,20);
            this.LabelDisabledForeColor = Color.FromArgb(235,245,255);
            this.LabelDisabledBorderColor = Color.FromArgb(60,70,100);
        }
    }
}