using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(245,246,248);
            this.LabelForeColor = Color.FromArgb(32,32,32);
            this.LabelBorderColor = Color.FromArgb(218,223,230);
            this.LabelHoverBorderColor = Color.FromArgb(218,223,230);
            this.LabelHoverBackColor = Color.FromArgb(245,246,248);
            this.LabelHoverForeColor = Color.FromArgb(32,32,32);
            this.LabelSelectedBorderColor = Color.FromArgb(218,223,230);
            this.LabelSelectedBackColor = Color.FromArgb(245,246,248);
            this.LabelSelectedForeColor = Color.FromArgb(32,32,32);
            this.LabelDisabledBackColor = Color.FromArgb(245,246,248);
            this.LabelDisabledForeColor = Color.FromArgb(32,32,32);
            this.LabelDisabledBorderColor = Color.FromArgb(218,223,230);
        }
    }
}