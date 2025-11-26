using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = SurfaceColor;
            this.LabelForeColor = ForeColor;
            this.LabelBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.LabelHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.LabelHoverBackColor = SurfaceColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.LabelSelectedBackColor = SurfaceColor;
            this.LabelSelectedForeColor = ForeColor;
            this.LabelDisabledBackColor = SurfaceColor;
            this.LabelDisabledForeColor = ThemeUtil.Lighten(ForeColor, -0.15);
            this.LabelDisabledBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
        }
    }
}
