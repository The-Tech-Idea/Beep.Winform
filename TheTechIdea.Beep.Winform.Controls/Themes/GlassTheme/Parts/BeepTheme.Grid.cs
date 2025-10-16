using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(200, 255, 255, 255);
            this.GridForeColor = Color.FromArgb(17,24,39);
            this.GridHeaderBackColor = Color.FromArgb(200, 255, 255, 255);
            this.GridHeaderForeColor = Color.FromArgb(17,24,39);
            this.GridHeaderBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.GridHeaderHoverBackColor = Color.FromArgb(200, 255, 255, 255);
            this.GridHeaderHoverForeColor = Color.FromArgb(17,24,39);
            this.GridHeaderSelectedBackColor = Color.FromArgb(200, 255, 255, 255);
            this.GridHeaderSelectedForeColor = Color.FromArgb(17,24,39);
            this.GridHeaderHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.GridRowHoverBackColor = Color.FromArgb(200, 255, 255, 255);
            this.GridRowHoverForeColor = Color.FromArgb(17,24,39);
            this.GridRowSelectedBackColor = Color.FromArgb(200, 255, 255, 255);
            this.GridRowSelectedForeColor = Color.FromArgb(17,24,39);
            this.GridRowHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.GridRowSelectedBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.GridLineColor = Color.FromArgb(120, 255, 255, 255);
        }
    }
}