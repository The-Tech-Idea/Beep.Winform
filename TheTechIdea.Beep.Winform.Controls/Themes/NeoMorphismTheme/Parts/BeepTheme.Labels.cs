using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = PanelBackColor;
            this.LabelForeColor = ForeColor;
            this.LabelBorderColor = BorderColor;
            this.LabelHoverBorderColor = ActiveBorderColor;
            this.LabelHoverBackColor = PanelGradiantMiddleColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = ActiveBorderColor;
            this.LabelSelectedBackColor = PanelGradiantMiddleColor;
            this.LabelSelectedForeColor = ForeColor;
            this.LabelDisabledBackColor = SurfaceColor;
            this.LabelDisabledForeColor = ForeColor;
            this.LabelDisabledBorderColor = InactiveBorderColor;
        }
    }
}