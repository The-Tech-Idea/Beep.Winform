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
            this.GridBackColor = Color.FromArgb(255,255,255);
            this.GridForeColor = Color.FromArgb(32,31,30);
            this.GridHeaderBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderForeColor = Color.FromArgb(32,31,30);
            this.GridHeaderBorderColor = Color.FromArgb(220,220,220);
            this.GridHeaderHoverBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderHoverForeColor = Color.FromArgb(32,31,30);
            this.GridHeaderSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderSelectedForeColor = Color.FromArgb(32,31,30);
            this.GridHeaderHoverBorderColor = Color.FromArgb(220,220,220);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(220,220,220);
            this.GridRowHoverBackColor = Color.FromArgb(255,255,255);
            this.GridRowHoverForeColor = Color.FromArgb(32,31,30);
            this.GridRowSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridRowSelectedForeColor = Color.FromArgb(32,31,30);
            this.GridRowHoverBorderColor = Color.FromArgb(220,220,220);
            this.GridRowSelectedBorderColor = Color.FromArgb(220,220,220);
            this.GridLineColor = Color.FromArgb(228, 228, 228);
        }
    }
}