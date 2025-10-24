using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(250,250,252);
            this.LabelForeColor = Color.FromArgb(28,28,30);
            this.LabelBorderColor = Color.FromArgb(229,229,234);
            this.LabelHoverBorderColor = Color.FromArgb(229,229,234);
            this.LabelHoverBackColor = Color.FromArgb(250,250,252);
            this.LabelHoverForeColor = Color.FromArgb(28,28,30);
            this.LabelSelectedBorderColor = Color.FromArgb(229,229,234);
            this.LabelSelectedBackColor = Color.FromArgb(250,250,252);
            this.LabelSelectedForeColor = Color.FromArgb(28,28,30);
            this.LabelDisabledBackColor = Color.FromArgb(250,250,252);
            this.LabelDisabledForeColor = Color.FromArgb(28,28,30);
            this.LabelDisabledBorderColor = Color.FromArgb(229,229,234);
        }
    }
}