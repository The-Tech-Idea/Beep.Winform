using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(255,251,235);
            this.LabelForeColor = Color.FromArgb(33,37,41);
            this.LabelBorderColor = Color.FromArgb(247,208,136);
            this.LabelHoverBorderColor = Color.FromArgb(247,208,136);
            this.LabelHoverBackColor = Color.FromArgb(255,251,235);
            this.LabelHoverForeColor = Color.FromArgb(33,37,41);
            this.LabelSelectedBorderColor = Color.FromArgb(247,208,136);
            this.LabelSelectedBackColor = Color.FromArgb(255,251,235);
            this.LabelSelectedForeColor = Color.FromArgb(33,37,41);
            this.LabelDisabledBackColor = Color.FromArgb(255,251,235);
            this.LabelDisabledForeColor = Color.FromArgb(33,37,41);
            this.LabelDisabledBorderColor = Color.FromArgb(247,208,136);
        }
    }
}