using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = SurfaceColor;
            this.LabelForeColor = ForeColor;
            this.LabelBorderColor = BorderColor;
            this.LabelHoverBorderColor = BorderColor;
            this.LabelHoverBackColor = SurfaceColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = BorderColor;
            this.LabelSelectedBackColor = SurfaceColor;
            this.LabelSelectedForeColor = ForeColor;
            this.LabelDisabledBackColor = SurfaceColor;
            this.LabelDisabledForeColor = ForeColor;
            this.LabelDisabledBorderColor = BorderColor;
        }
    }
}