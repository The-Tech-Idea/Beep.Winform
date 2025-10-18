using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(15,16,32);
            this.LabelForeColor = Color.FromArgb(245,247,255);
            this.LabelBorderColor = Color.FromArgb(74,79,123);
            this.LabelHoverBorderColor = Color.FromArgb(74,79,123);
            this.LabelHoverBackColor = Color.FromArgb(15,16,32);
            this.LabelHoverForeColor = Color.FromArgb(245,247,255);
            this.LabelSelectedBorderColor = Color.FromArgb(74,79,123);
            this.LabelSelectedBackColor = Color.FromArgb(15,16,32);
            this.LabelSelectedForeColor = Color.FromArgb(245,247,255);
            this.LabelDisabledBackColor = Color.FromArgb(15,16,32);
            this.LabelDisabledForeColor = Color.FromArgb(245,247,255);
            this.LabelDisabledBorderColor = Color.FromArgb(74,79,123);
        }
    }
}