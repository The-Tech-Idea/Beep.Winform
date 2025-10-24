using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = Color.FromArgb(245,248,255);
            this.LabelForeColor = Color.FromArgb(24,28,35);
            this.LabelBorderColor = Color.FromArgb(210,220,235);
            this.LabelHoverBorderColor = Color.FromArgb(210,220,235);
            this.LabelHoverBackColor = Color.FromArgb(245,248,255);
            this.LabelHoverForeColor = Color.FromArgb(24,28,35);
            this.LabelSelectedBorderColor = Color.FromArgb(210,220,235);
            this.LabelSelectedBackColor = Color.FromArgb(245,248,255);
            this.LabelSelectedForeColor = Color.FromArgb(24,28,35);
            this.LabelDisabledBackColor = Color.FromArgb(245,248,255);
            this.LabelDisabledForeColor = Color.FromArgb(24,28,35);
            this.LabelDisabledBorderColor = Color.FromArgb(210,220,235);
        }
    }
}