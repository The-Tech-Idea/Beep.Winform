using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = PanelBackColor;
            this.GridForeColor = ForeColor;
            this.GridHeaderBackColor = PanelBackColor;
            this.GridHeaderForeColor = ForeColor;
            this.GridHeaderBorderColor = SecondaryColor;
            this.GridHeaderHoverBackColor = PanelGradiantMiddleColor;
            this.GridHeaderHoverForeColor = ForeColor;
            this.GridHeaderSelectedBackColor = PanelGradiantMiddleColor;
            this.GridHeaderSelectedForeColor = ForeColor;
            this.GridHeaderHoverBorderColor = SecondaryColor;
            this.GridHeaderSelectedBorderColor = SecondaryColor;
            this.GridRowHoverBackColor = PanelGradiantMiddleColor;
            this.GridRowHoverForeColor = ForeColor;
            this.GridRowSelectedBackColor = PanelBackColor;
            this.GridRowSelectedForeColor = ForeColor;
            this.GridRowHoverBorderColor = SecondaryColor;
            this.GridRowSelectedBorderColor = SecondaryColor;
            this.GridLineColor = SecondaryColor;
        }
    }
}