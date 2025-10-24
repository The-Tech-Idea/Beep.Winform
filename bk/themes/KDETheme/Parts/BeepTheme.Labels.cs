using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(248,249,250);
            this.LabelForeColor = Color.FromArgb(33,37,41);
            this.LabelBorderColor = Color.FromArgb(222,226,230);
            this.LabelHoverBorderColor = Color.FromArgb(222,226,230);
            this.LabelHoverBackColor = Color.FromArgb(248,249,250);
            this.LabelHoverForeColor = Color.FromArgb(33,37,41);
            this.LabelSelectedBorderColor = Color.FromArgb(222,226,230);
            this.LabelSelectedBackColor = Color.FromArgb(248,249,250);
            this.LabelSelectedForeColor = Color.FromArgb(33,37,41);
            this.LabelDisabledBackColor = Color.FromArgb(248,249,250);
            this.LabelDisabledForeColor = Color.FromArgb(33,37,41);
            this.LabelDisabledBorderColor = Color.FromArgb(222,226,230);
        }
    }
}