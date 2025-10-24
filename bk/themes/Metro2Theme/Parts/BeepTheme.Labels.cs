using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(243,242,241);
            this.LabelForeColor = Color.FromArgb(32,31,30);
            this.LabelBorderColor = Color.FromArgb(220,220,220);
            this.LabelHoverBorderColor = Color.FromArgb(220,220,220);
            this.LabelHoverBackColor = Color.FromArgb(243,242,241);
            this.LabelHoverForeColor = Color.FromArgb(32,31,30);
            this.LabelSelectedBorderColor = Color.FromArgb(220,220,220);
            this.LabelSelectedBackColor = Color.FromArgb(243,242,241);
            this.LabelSelectedForeColor = Color.FromArgb(32,31,30);
            this.LabelDisabledBackColor = Color.FromArgb(243,242,241);
            this.LabelDisabledForeColor = Color.FromArgb(32,31,30);
            this.LabelDisabledBorderColor = Color.FromArgb(220,220,220);
        }
    }
}