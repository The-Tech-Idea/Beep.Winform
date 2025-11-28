using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = SurfaceColor;
            this.GridForeColor = ForeColor;
            this.GridHeaderBackColor = SurfaceColor;
            this.GridHeaderForeColor = ForeColor;
            this.GridHeaderBorderColor = BorderColor;
            this.GridHeaderHoverBackColor = SurfaceColor;
            this.GridHeaderHoverForeColor = ForeColor;
            this.GridHeaderSelectedBackColor = SurfaceColor;
            this.GridHeaderSelectedForeColor = ForeColor;
            this.GridHeaderHoverBorderColor = BorderColor;
            this.GridHeaderSelectedBorderColor = BorderColor;
            this.GridRowHoverBackColor = SurfaceColor;
            this.GridRowHoverForeColor = ForeColor;
            this.GridRowSelectedBackColor = SurfaceColor;
            this.GridRowSelectedForeColor = ForeColor;
            this.GridRowHoverBorderColor = BorderColor;
            this.GridRowSelectedBorderColor = BorderColor;
            this.GridLineColor = BorderColor;
        }
    }
}