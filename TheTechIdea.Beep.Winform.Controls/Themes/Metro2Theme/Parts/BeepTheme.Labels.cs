using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = PanelBackColor;
            this.LabelForeColor = ForeColor;
            this.LabelBorderColor = InactiveBorderColor;
            this.LabelHoverBorderColor = InactiveBorderColor;
            this.LabelHoverBackColor = PanelGradiantMiddleColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = InactiveBorderColor;
            this.LabelSelectedBackColor = PanelBackColor;
            this.LabelSelectedForeColor = ForeColor;
            this.LabelDisabledBackColor = PanelBackColor;
            this.LabelDisabledForeColor = ForeColor;
            this.LabelDisabledBorderColor = InactiveBorderColor;
        }
    }
}