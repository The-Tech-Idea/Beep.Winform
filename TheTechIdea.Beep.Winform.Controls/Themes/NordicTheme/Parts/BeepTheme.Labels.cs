using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(250,250,251);
            this.LabelForeColor = Color.FromArgb(31,41,55);
            this.LabelBorderColor = Color.FromArgb(229,231,235);
            this.LabelHoverBorderColor = Color.FromArgb(229,231,235);
            this.LabelHoverBackColor = Color.FromArgb(250,250,251);
            this.LabelHoverForeColor = Color.FromArgb(31,41,55);
            this.LabelSelectedBorderColor = Color.FromArgb(229,231,235);
            this.LabelSelectedBackColor = Color.FromArgb(250,250,251);
            this.LabelSelectedForeColor = Color.FromArgb(31,41,55);
            this.LabelDisabledBackColor = Color.FromArgb(250,250,251);
            this.LabelDisabledForeColor = Color.FromArgb(31,41,55);
            this.LabelDisabledBorderColor = Color.FromArgb(229,231,235);
        }
    }
}