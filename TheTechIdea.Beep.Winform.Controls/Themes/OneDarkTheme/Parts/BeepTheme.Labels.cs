using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = PanelBackColor;
            this.LabelForeColor = ForeColor;
            this.LabelBorderColor = BorderColor;
            this.LabelHoverBorderColor = BorderColor;
            this.LabelHoverBackColor = PanelGradiantMiddleColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = ActiveBorderColor;
            this.LabelSelectedBackColor = PanelGradiantMiddleColor;
            this.LabelSelectedForeColor = ForeColor;
            this.LabelDisabledBackColor = PanelBackColor;
            this.LabelDisabledForeColor = InactiveBorderColor;
            this.LabelDisabledBorderColor = InactiveBorderColor;
        }
    }
}