using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = BackgroundColor;
            this.GridForeColor = ForeColor;
            this.GridHeaderBackColor = BackgroundColor;
            this.GridHeaderForeColor = ForeColor;
            this.GridHeaderBorderColor = InactiveBorderColor;
            this.GridHeaderHoverBackColor = BackgroundColor;
            this.GridHeaderHoverForeColor = ForeColor;
            this.GridHeaderSelectedBackColor = BackgroundColor;
            this.GridHeaderSelectedForeColor = ForeColor;
            this.GridHeaderHoverBorderColor = InactiveBorderColor;
            this.GridHeaderSelectedBorderColor = InactiveBorderColor;
            this.GridRowHoverBackColor = BackgroundColor;
            this.GridRowHoverForeColor = ForeColor;
            this.GridRowSelectedBackColor = BackgroundColor;
            this.GridRowSelectedForeColor = ForeColor;
            this.GridRowHoverBorderColor = InactiveBorderColor;
            this.GridRowSelectedBorderColor = InactiveBorderColor;
            this.GridLineColor = InactiveBorderColor;
        }
    }
}