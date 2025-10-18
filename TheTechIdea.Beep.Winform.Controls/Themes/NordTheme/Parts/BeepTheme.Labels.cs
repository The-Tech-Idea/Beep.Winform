using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(46,52,64);
            this.LabelForeColor = Color.FromArgb(216,222,233);
            this.LabelBorderColor = Color.FromArgb(76,86,106);
            this.LabelHoverBorderColor = Color.FromArgb(76,86,106);
            this.LabelHoverBackColor = Color.FromArgb(46,52,64);
            this.LabelHoverForeColor = Color.FromArgb(216,222,233);
            this.LabelSelectedBorderColor = Color.FromArgb(76,86,106);
            this.LabelSelectedBackColor = Color.FromArgb(46,52,64);
            this.LabelSelectedForeColor = Color.FromArgb(216,222,233);
            this.LabelDisabledBackColor = Color.FromArgb(46,52,64);
            this.LabelDisabledForeColor = Color.FromArgb(216,222,233);
            this.LabelDisabledBorderColor = Color.FromArgb(76,86,106);
        }
    }
}