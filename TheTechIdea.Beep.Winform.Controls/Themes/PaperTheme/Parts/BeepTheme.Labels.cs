using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(250,250,250);
            this.LabelForeColor = Color.FromArgb(33,33,33);
            this.LabelBorderColor = Color.FromArgb(224,224,224);
            this.LabelHoverBorderColor = Color.FromArgb(224,224,224);
            this.LabelHoverBackColor = Color.FromArgb(250,250,250);
            this.LabelHoverForeColor = Color.FromArgb(33,33,33);
            this.LabelSelectedBorderColor = Color.FromArgb(224,224,224);
            this.LabelSelectedBackColor = Color.FromArgb(250,250,250);
            this.LabelSelectedForeColor = Color.FromArgb(33,33,33);
            this.LabelDisabledBackColor = Color.FromArgb(250,250,250);
            this.LabelDisabledForeColor = Color.FromArgb(33,33,33);
            this.LabelDisabledBorderColor = Color.FromArgb(224,224,224);
        }
    }
}