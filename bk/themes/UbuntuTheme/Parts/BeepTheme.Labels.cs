using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(242,242,245);
            this.LabelForeColor = Color.FromArgb(44,44,44);
            this.LabelBorderColor = Color.FromArgb(218,218,222);
            this.LabelHoverBorderColor = Color.FromArgb(218,218,222);
            this.LabelHoverBackColor = Color.FromArgb(242,242,245);
            this.LabelHoverForeColor = Color.FromArgb(44,44,44);
            this.LabelSelectedBorderColor = Color.FromArgb(218,218,222);
            this.LabelSelectedBackColor = Color.FromArgb(242,242,245);
            this.LabelSelectedForeColor = Color.FromArgb(44,44,44);
            this.LabelDisabledBackColor = Color.FromArgb(242,242,245);
            this.LabelDisabledForeColor = Color.FromArgb(44,44,44);
            this.LabelDisabledBorderColor = Color.FromArgb(218,218,222);
        }
    }
}