using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = BackgroundColor;
            this.LabelForeColor = ThemeUtil.Darken(ForeColor, 0.25);
            this.LabelBorderColor = BorderColor;
            this.LabelHoverBorderColor = BorderColor;
            this.LabelHoverBackColor = SurfaceColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = ActiveBorderColor;
            this.LabelSelectedBackColor = SurfaceColor;
            this.LabelSelectedForeColor = OnPrimaryColor;
            this.LabelDisabledBackColor = BackgroundColor;
            this.LabelDisabledForeColor = ThemeUtil.Darken(ForeColor, 0.4);
            this.LabelDisabledBorderColor = BorderColor;
        }
    }
}