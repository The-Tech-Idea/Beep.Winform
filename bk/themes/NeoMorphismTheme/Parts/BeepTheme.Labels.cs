using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(236,240,243);
            this.LabelForeColor = Color.FromArgb(58,66,86);
            this.LabelBorderColor = Color.FromArgb(221,228,235);
            this.LabelHoverBorderColor = Color.FromArgb(221,228,235);
            this.LabelHoverBackColor = Color.FromArgb(236,240,243);
            this.LabelHoverForeColor = Color.FromArgb(58,66,86);
            this.LabelSelectedBorderColor = Color.FromArgb(221,228,235);
            this.LabelSelectedBackColor = Color.FromArgb(236,240,243);
            this.LabelSelectedForeColor = Color.FromArgb(58,66,86);
            this.LabelDisabledBackColor = Color.FromArgb(236,240,243);
            this.LabelDisabledForeColor = Color.FromArgb(58,66,86);
            this.LabelDisabledBorderColor = Color.FromArgb(221,228,235);
        }
    }
}