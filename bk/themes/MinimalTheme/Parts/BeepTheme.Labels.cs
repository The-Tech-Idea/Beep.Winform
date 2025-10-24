using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(255,255,255);
            this.LabelForeColor = Color.FromArgb(31,41,55);
            this.LabelBorderColor = Color.FromArgb(209,213,219);
            this.LabelHoverBorderColor = Color.FromArgb(209,213,219);
            this.LabelHoverBackColor = Color.FromArgb(255,255,255);
            this.LabelHoverForeColor = Color.FromArgb(31,41,55);
            this.LabelSelectedBorderColor = Color.FromArgb(209,213,219);
            this.LabelSelectedBackColor = Color.FromArgb(255,255,255);
            this.LabelSelectedForeColor = Color.FromArgb(31,41,55);
            this.LabelDisabledBackColor = Color.FromArgb(255,255,255);
            this.LabelDisabledForeColor = Color.FromArgb(31,41,55);
            this.LabelDisabledBorderColor = Color.FromArgb(209,213,219);
        }
    }
}