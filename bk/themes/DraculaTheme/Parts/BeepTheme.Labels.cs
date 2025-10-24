using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(40,42,54);
            this.LabelForeColor = Color.FromArgb(248,248,242);
            this.LabelBorderColor = Color.FromArgb(98,114,164);
            this.LabelHoverBorderColor = Color.FromArgb(98,114,164);
            this.LabelHoverBackColor = Color.FromArgb(40,42,54);
            this.LabelHoverForeColor = Color.FromArgb(248,248,242);
            this.LabelSelectedBorderColor = Color.FromArgb(98,114,164);
            this.LabelSelectedBackColor = Color.FromArgb(40,42,54);
            this.LabelSelectedForeColor = Color.FromArgb(248,248,242);
            this.LabelDisabledBackColor = Color.FromArgb(40,42,54);
            this.LabelDisabledForeColor = Color.FromArgb(248,248,242);
            this.LabelDisabledBorderColor = Color.FromArgb(98,114,164);
        }
    }
}