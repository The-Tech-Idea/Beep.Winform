using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(242,242,247);
            this.LabelForeColor = Color.FromArgb(28,28,30);
            this.LabelBorderColor = Color.FromArgb(198,198,207);
            this.LabelHoverBorderColor = Color.FromArgb(198,198,207);
            this.LabelHoverBackColor = Color.FromArgb(242,242,247);
            this.LabelHoverForeColor = Color.FromArgb(28,28,30);
            this.LabelSelectedBorderColor = Color.FromArgb(198,198,207);
            this.LabelSelectedBackColor = Color.FromArgb(242,242,247);
            this.LabelSelectedForeColor = Color.FromArgb(28,28,30);
            this.LabelDisabledBackColor = Color.FromArgb(242,242,247);
            this.LabelDisabledForeColor = Color.FromArgb(28,28,30);
            this.LabelDisabledBorderColor = Color.FromArgb(198,198,207);
        }
    }
}