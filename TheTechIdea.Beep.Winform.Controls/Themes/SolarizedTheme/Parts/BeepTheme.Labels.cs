using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(0,43,54);
            this.LabelForeColor = Color.FromArgb(147,161,161);
            this.LabelBorderColor = Color.FromArgb(88,110,117);
            this.LabelHoverBorderColor = Color.FromArgb(88,110,117);
            this.LabelHoverBackColor = Color.FromArgb(0,43,54);
            this.LabelHoverForeColor = Color.FromArgb(147,161,161);
            this.LabelSelectedBorderColor = Color.FromArgb(88,110,117);
            this.LabelSelectedBackColor = Color.FromArgb(0,43,54);
            this.LabelSelectedForeColor = Color.FromArgb(147,161,161);
            this.LabelDisabledBackColor = Color.FromArgb(0,43,54);
            this.LabelDisabledForeColor = Color.FromArgb(147,161,161);
            this.LabelDisabledBorderColor = Color.FromArgb(88,110,117);
        }
    }
}