using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(250,250,250);
            this.LabelForeColor = Color.FromArgb(20,20,20);
            this.LabelBorderColor = Color.FromArgb(0,0,0);
            this.LabelHoverBorderColor = Color.FromArgb(0,0,0);
            this.LabelHoverBackColor = Color.FromArgb(250,250,250);
            this.LabelHoverForeColor = Color.FromArgb(20,20,20);
            this.LabelSelectedBorderColor = Color.FromArgb(0,0,0);
            this.LabelSelectedBackColor = Color.FromArgb(250,250,250);
            this.LabelSelectedForeColor = Color.FromArgb(20,20,20);
            this.LabelDisabledBackColor = Color.FromArgb(250,250,250);
            this.LabelDisabledForeColor = Color.FromArgb(20,20,20);
            this.LabelDisabledBorderColor = Color.FromArgb(0,0,0);
        }
    }
}