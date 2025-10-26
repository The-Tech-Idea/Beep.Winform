using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = ForeColor;
            this.GridForeColor = ForeColor;
            this.GridHeaderBackColor = ForeColor;
            this.GridHeaderForeColor = ForeColor;
            this.GridHeaderBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.GridHeaderHoverBackColor = ForeColor;
            this.GridHeaderHoverForeColor = ForeColor;
            this.GridHeaderSelectedBackColor = ForeColor;
            this.GridHeaderSelectedForeColor = ForeColor;
            this.GridHeaderHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.GridHeaderSelectedBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.GridRowHoverBackColor = ForeColor;
            this.GridRowHoverForeColor = ForeColor;
            this.GridRowSelectedBackColor = ForeColor;
            this.GridRowSelectedForeColor = ForeColor;
            this.GridRowHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.GridRowSelectedBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.GridLineColor = ThemeUtil.Lighten(BackgroundColor, 0.22);
        }
    }
}
