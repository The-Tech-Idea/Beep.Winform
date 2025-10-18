using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(18,20,30);
            this.GridForeColor = Color.FromArgb(235,245,255);
            this.GridHeaderBackColor = Color.FromArgb(18,20,30);
            this.GridHeaderForeColor = Color.FromArgb(235,245,255);
            this.GridHeaderBorderColor = Color.FromArgb(60,70,100);
            this.GridHeaderHoverBackColor = Color.FromArgb(18,20,30);
            this.GridHeaderHoverForeColor = Color.FromArgb(235,245,255);
            this.GridHeaderSelectedBackColor = Color.FromArgb(18,20,30);
            this.GridHeaderSelectedForeColor = Color.FromArgb(235,245,255);
            this.GridHeaderHoverBorderColor = Color.FromArgb(60,70,100);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(60,70,100);
            this.GridRowHoverBackColor = Color.FromArgb(18,20,30);
            this.GridRowHoverForeColor = Color.FromArgb(235,245,255);
            this.GridRowSelectedBackColor = Color.FromArgb(18,20,30);
            this.GridRowSelectedForeColor = Color.FromArgb(235,245,255);
            this.GridRowHoverBorderColor = Color.FromArgb(60,70,100);
            this.GridRowSelectedBorderColor = Color.FromArgb(60,70,100);
            this.GridLineColor = Color.FromArgb(60,70,100);
        }
    }
}