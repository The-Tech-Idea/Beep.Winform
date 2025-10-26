using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = BackgroundColor;
            this.LabelForeColor = ForeColor;
            this.LabelBorderColor = BorderColor;
            this.LabelHoverBorderColor = ActiveBorderColor;
            this.LabelHoverBackColor = SurfaceColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = ActiveBorderColor;
            this.LabelSelectedBackColor = SurfaceColor;
            this.LabelSelectedForeColor = ForeColor;
            this.LabelDisabledBackColor = BackgroundColor;
            this.LabelDisabledForeColor = InactiveBorderColor;
            this.LabelDisabledBorderColor = InactiveBorderColor;
        }
    }
}
