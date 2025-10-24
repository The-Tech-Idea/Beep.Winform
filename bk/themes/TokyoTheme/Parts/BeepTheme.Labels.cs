using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(26,27,38);
            this.LabelForeColor = Color.FromArgb(192,202,245);
            this.LabelBorderColor = Color.FromArgb(86,95,137);
            this.LabelHoverBorderColor = Color.FromArgb(86,95,137);
            this.LabelHoverBackColor = Color.FromArgb(26,27,38);
            this.LabelHoverForeColor = Color.FromArgb(192,202,245);
            this.LabelSelectedBorderColor = Color.FromArgb(86,95,137);
            this.LabelSelectedBackColor = Color.FromArgb(26,27,38);
            this.LabelSelectedForeColor = Color.FromArgb(192,202,245);
            this.LabelDisabledBackColor = Color.FromArgb(26,27,38);
            this.LabelDisabledForeColor = Color.FromArgb(192,202,245);
            this.LabelDisabledBorderColor = Color.FromArgb(86,95,137);
        }
    }
}