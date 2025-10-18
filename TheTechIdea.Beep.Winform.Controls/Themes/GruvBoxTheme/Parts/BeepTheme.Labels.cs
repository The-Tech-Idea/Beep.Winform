using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(40,40,40);
            this.LabelForeColor = Color.FromArgb(235,219,178);
            this.LabelBorderColor = Color.FromArgb(168,153,132);
            this.LabelHoverBorderColor = Color.FromArgb(168,153,132);
            this.LabelHoverBackColor = Color.FromArgb(40,40,40);
            this.LabelHoverForeColor = Color.FromArgb(235,219,178);
            this.LabelSelectedBorderColor = Color.FromArgb(168,153,132);
            this.LabelSelectedBackColor = Color.FromArgb(40,40,40);
            this.LabelSelectedForeColor = Color.FromArgb(235,219,178);
            this.LabelDisabledBackColor = Color.FromArgb(40,40,40);
            this.LabelDisabledForeColor = Color.FromArgb(235,219,178);
            this.LabelDisabledBorderColor = Color.FromArgb(168,153,132);
        }
    }
}