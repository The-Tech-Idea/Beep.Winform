using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(246,245,244);
            this.LabelForeColor = Color.FromArgb(46,52,54);
            this.LabelBorderColor = Color.FromArgb(205,207,212);
            this.LabelHoverBorderColor = Color.FromArgb(205,207,212);
            this.LabelHoverBackColor = Color.FromArgb(246,245,244);
            this.LabelHoverForeColor = Color.FromArgb(46,52,54);
            this.LabelSelectedBorderColor = Color.FromArgb(205,207,212);
            this.LabelSelectedBackColor = Color.FromArgb(246,245,244);
            this.LabelSelectedForeColor = Color.FromArgb(46,52,54);
            this.LabelDisabledBackColor = Color.FromArgb(246,245,244);
            this.LabelDisabledForeColor = Color.FromArgb(46,52,54);
            this.LabelDisabledBorderColor = Color.FromArgb(205,207,212);
        }
    }
}