using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = SurfaceColor;
            this.GridForeColor = ForeColor;
            this.GridHeaderBackColor = SurfaceColor;
            this.GridHeaderForeColor = ForeColor;
            this.GridHeaderBorderColor = BorderColor;
            this.GridHeaderHoverBackColor = PanelGradiantMiddleColor;
            this.GridHeaderHoverForeColor = ForeColor;
            this.GridHeaderSelectedBackColor = PanelGradiantMiddleColor;
            this.GridHeaderSelectedForeColor = ForeColor;
            this.GridHeaderHoverBorderColor = ActiveBorderColor;
            this.GridHeaderSelectedBorderColor = ActiveBorderColor;
            this.GridRowHoverBackColor = PanelGradiantMiddleColor;
            this.GridRowHoverForeColor = ForeColor;
            this.GridRowSelectedBackColor = PanelGradiantMiddleColor;
            this.GridRowSelectedForeColor = ForeColor;
            this.GridRowHoverBorderColor = ActiveBorderColor;
            this.GridRowSelectedBorderColor = ActiveBorderColor;
            this.GridLineColor = InactiveBorderColor;
        }
    }
}