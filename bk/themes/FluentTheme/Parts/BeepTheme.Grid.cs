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
            this.GridBackColor = Color.FromArgb(255,255,255);
            this.GridForeColor = Color.FromArgb(32,32,32);
            this.GridHeaderBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderForeColor = Color.FromArgb(32,32,32);
            this.GridHeaderBorderColor = Color.FromArgb(218,223,230);
            this.GridHeaderHoverBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderHoverForeColor = Color.FromArgb(32,32,32);
            this.GridHeaderSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderSelectedForeColor = Color.FromArgb(32,32,32);
            this.GridHeaderHoverBorderColor = Color.FromArgb(218,223,230);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(218,223,230);
            this.GridRowHoverBackColor = Color.FromArgb(255,255,255);
            this.GridRowHoverForeColor = Color.FromArgb(32,32,32);
            this.GridRowSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridRowSelectedForeColor = Color.FromArgb(32,32,32);
            this.GridRowHoverBorderColor = Color.FromArgb(218,223,230);
            this.GridRowSelectedBorderColor = Color.FromArgb(218,223,230);
            this.GridLineColor = Color.FromArgb(229,231,235);
        }
    }
}