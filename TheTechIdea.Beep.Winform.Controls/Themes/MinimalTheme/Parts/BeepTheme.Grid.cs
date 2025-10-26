using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = SurfaceColor;
            this.GridForeColor = ForeColor;
            this.GridHeaderBackColor = BackgroundColor;
            this.GridHeaderForeColor = ForeColor;
            this.GridHeaderBorderColor = BorderColor;
            this.GridHeaderHoverBackColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.GridHeaderHoverForeColor = ForeColor;
            this.GridHeaderSelectedBackColor = SurfaceColor;
            this.GridHeaderSelectedForeColor = ForeColor;
            this.GridHeaderHoverBorderColor = ActiveBorderColor;
            this.GridHeaderSelectedBorderColor = ActiveBorderColor;
            this.GridRowHoverBackColor = ThemeUtil.Lighten(SurfaceColor, 0.02);
            this.GridRowHoverForeColor = ForeColor;
            this.GridRowSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.05);
            this.GridRowSelectedForeColor = ForeColor;
            this.GridRowHoverBorderColor = ActiveBorderColor;
            this.GridRowSelectedBorderColor = ActiveBorderColor;
            this.GridLineColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
        }
    }
}
