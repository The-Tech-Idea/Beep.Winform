using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = BackgroundColor;
            this.LabelForeColor = ForeColor;
            this.LabelBorderColor = SecondaryColor;
            this.LabelHoverBorderColor = SecondaryColor;
            this.LabelHoverBackColor = PanelBackColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = SecondaryColor;
            this.LabelSelectedBackColor = PanelBackColor;
            this.LabelSelectedForeColor = ForeColor;
            this.LabelDisabledBackColor = BackgroundColor;
            this.LabelDisabledForeColor = InactiveBorderColor;
            this.LabelDisabledBorderColor = SecondaryColor;
        }
    }
}