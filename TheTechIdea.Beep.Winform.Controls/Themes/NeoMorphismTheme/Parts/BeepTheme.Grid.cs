using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.GridHeaderBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.GridHeaderHoverBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.GridRowHoverBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
              this.GridForeColor = ForeColor;
              this.GridHeaderForeColor = ForeColor;
              this.GridHeaderBorderColor = BorderColor;
              this.GridHeaderHoverForeColor = ForeColor;
              this.GridHeaderSelectedForeColor = ForeColor;
              this.GridHeaderHoverBorderColor = ActiveBorderColor;
              this.GridHeaderSelectedBorderColor = ActiveBorderColor;
              this.GridRowHoverForeColor = ForeColor;
              this.GridRowSelectedForeColor = ForeColor;
              this.GridRowHoverBorderColor = ActiveBorderColor;
              this.GridRowSelectedBorderColor = ActiveBorderColor;
              this.GridLineColor = BorderColor;
        }
    }
}