using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = PanelGradiantMiddleColor;
            this.LabelForeColor = ForeColor;
            this.LabelBorderColor = InactiveBorderColor;
            this.LabelHoverBorderColor = InactiveBorderColor;
            this.LabelHoverBackColor = PanelGradiantMiddleColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = InactiveBorderColor;
            this.LabelSelectedBackColor = PanelGradiantMiddleColor;
            this.LabelSelectedForeColor = ForeColor;
            this.LabelDisabledBackColor = PanelGradiantMiddleColor;
            this.LabelDisabledForeColor = ForeColor;
            this.LabelDisabledBorderColor = InactiveBorderColor;
        }
    }
}