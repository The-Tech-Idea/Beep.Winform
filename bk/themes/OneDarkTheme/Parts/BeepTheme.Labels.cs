using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(40,44,52);
            this.LabelForeColor = Color.FromArgb(171,178,191);
            this.LabelBorderColor = Color.FromArgb(92,99,112);
            this.LabelHoverBorderColor = Color.FromArgb(92,99,112);
            this.LabelHoverBackColor = Color.FromArgb(40,44,52);
            this.LabelHoverForeColor = Color.FromArgb(171,178,191);
            this.LabelSelectedBorderColor = Color.FromArgb(92,99,112);
            this.LabelSelectedBackColor = Color.FromArgb(40,44,52);
            this.LabelSelectedForeColor = Color.FromArgb(171,178,191);
            this.LabelDisabledBackColor = Color.FromArgb(40,44,52);
            this.LabelDisabledForeColor = Color.FromArgb(171,178,191);
            this.LabelDisabledBorderColor = Color.FromArgb(92,99,112);
        }
    }
}