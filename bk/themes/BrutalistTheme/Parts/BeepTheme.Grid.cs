using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(255,255,255);
            this.GridForeColor = Color.FromArgb(20,20,20);
            this.GridHeaderBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderForeColor = Color.FromArgb(20,20,20);
            this.GridHeaderBorderColor = Color.FromArgb(0,0,0);
            this.GridHeaderHoverBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderHoverForeColor = Color.FromArgb(20,20,20);
            this.GridHeaderSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderSelectedForeColor = Color.FromArgb(20,20,20);
            this.GridHeaderHoverBorderColor = Color.FromArgb(0,0,0);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(0,0,0);
            this.GridRowHoverBackColor = Color.FromArgb(255,255,255);
            this.GridRowHoverForeColor = Color.FromArgb(20,20,20);
            this.GridRowSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridRowSelectedForeColor = Color.FromArgb(20,20,20);
            this.GridRowHoverBorderColor = Color.FromArgb(0,0,0);
            this.GridRowSelectedBorderColor = Color.FromArgb(0,0,0);
            this.GridLineColor = Color.FromArgb(0,0,0);
        }
    }
}