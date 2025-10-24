using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(255,255,255);
            this.GridForeColor = Color.FromArgb(28,28,30);
            this.GridHeaderBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderForeColor = Color.FromArgb(28,28,30);
            this.GridHeaderBorderColor = Color.FromArgb(198,198,207);
            this.GridHeaderHoverBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderHoverForeColor = Color.FromArgb(28,28,30);
            this.GridHeaderSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderSelectedForeColor = Color.FromArgb(28,28,30);
            this.GridHeaderHoverBorderColor = Color.FromArgb(198,198,207);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(198,198,207);
            this.GridRowHoverBackColor = Color.FromArgb(255,255,255);
            this.GridRowHoverForeColor = Color.FromArgb(28,28,30);
            this.GridRowSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridRowSelectedForeColor = Color.FromArgb(28,28,30);
            this.GridRowHoverBorderColor = Color.FromArgb(198,198,207);
            this.GridRowSelectedBorderColor = Color.FromArgb(198,198,207);
            this.GridLineColor = Color.FromArgb(230,231,235);
        }
    }
}