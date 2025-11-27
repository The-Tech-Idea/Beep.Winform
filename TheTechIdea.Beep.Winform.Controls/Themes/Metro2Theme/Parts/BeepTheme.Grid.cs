using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = PanelBackColor;
            this.GridForeColor = ForeColor;
            this.GridHeaderBackColor = PanelBackColor;
            this.GridHeaderForeColor = ForeColor;
            this.GridHeaderBorderColor = InactiveBorderColor;
            this.GridHeaderHoverBackColor = PanelGradiantMiddleColor;
            this.GridHeaderHoverForeColor = ForeColor;
            this.GridHeaderSelectedBackColor = PanelBackColor;
            this.GridHeaderSelectedForeColor = ForeColor;
            this.GridHeaderHoverBorderColor = InactiveBorderColor;
            this.GridHeaderSelectedBorderColor = InactiveBorderColor;
            this.GridRowHoverBackColor = PanelGradiantMiddleColor;
            this.GridRowHoverForeColor = ForeColor;
            this.GridRowSelectedBackColor = PanelBackColor;
            this.GridRowSelectedForeColor = ForeColor;
            this.GridRowHoverBorderColor = InactiveBorderColor;
            this.GridRowSelectedBorderColor = InactiveBorderColor;
            this.GridLineColor = InactiveBorderColor;
        }
    }
}