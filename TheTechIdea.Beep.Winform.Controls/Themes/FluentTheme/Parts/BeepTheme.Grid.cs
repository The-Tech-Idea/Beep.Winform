using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = SurfaceColor;
            this.GridForeColor = ForeColor;
            this.GridHeaderBackColor = SurfaceColor;
            this.GridHeaderForeColor = ForeColor;
            this.GridHeaderBorderColor = BorderColor;
            this.GridHeaderHoverBackColor = SecondaryColor;
            this.GridHeaderHoverForeColor = ForeColor;
            this.GridHeaderSelectedBackColor = PrimaryColor;
            this.GridHeaderSelectedForeColor = OnPrimaryColor;
            this.GridHeaderHoverBorderColor = ActiveBorderColor;
            this.GridHeaderSelectedBorderColor = PrimaryColor;
            this.GridRowHoverBackColor = SecondaryColor;
            this.GridRowHoverForeColor = ForeColor;
            this.GridRowSelectedBackColor = PrimaryColor;
            this.GridRowSelectedForeColor = OnPrimaryColor;
            this.GridRowHoverBorderColor = ActiveBorderColor;
            this.GridRowSelectedBorderColor = PrimaryColor;
            this.GridLineColor = BorderColor;
        }
    }
}