using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(245,246,247);
            this.LabelForeColor = Color.FromArgb(43,45,48);
            this.LabelBorderColor = Color.FromArgb(220,223,230);
            this.LabelHoverBorderColor = Color.FromArgb(220,223,230);
            this.LabelHoverBackColor = Color.FromArgb(245,246,247);
            this.LabelHoverForeColor = Color.FromArgb(43,45,48);
            this.LabelSelectedBorderColor = Color.FromArgb(220,223,230);
            this.LabelSelectedBackColor = Color.FromArgb(245,246,247);
            this.LabelSelectedForeColor = Color.FromArgb(43,45,48);
            this.LabelDisabledBackColor = Color.FromArgb(245,246,247);
            this.LabelDisabledForeColor = Color.FromArgb(43,45,48);
            this.LabelDisabledBorderColor = Color.FromArgb(220,223,230);
        }
    }
}