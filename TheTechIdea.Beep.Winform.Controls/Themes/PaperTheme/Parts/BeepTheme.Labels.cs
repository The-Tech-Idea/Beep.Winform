using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = BackgroundColor;
            this.LabelForeColor = ForeColor;
            this.LabelBorderColor = InactiveBorderColor;
            this.LabelHoverBorderColor = InactiveBorderColor;
            this.LabelHoverBackColor = BackgroundColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = InactiveBorderColor;
            this.LabelSelectedBackColor = BackgroundColor;
            this.LabelSelectedForeColor = ForeColor;
            this.LabelDisabledBackColor = BackgroundColor;
            this.LabelDisabledForeColor = ForeColor;
            this.LabelDisabledBorderColor = InactiveBorderColor;
        }
    }
}