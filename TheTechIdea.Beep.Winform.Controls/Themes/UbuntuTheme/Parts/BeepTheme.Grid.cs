using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(255,255,255);
            this.GridForeColor = Color.FromArgb(44,44,44);
            this.GridHeaderBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderForeColor = Color.FromArgb(44,44,44);
            this.GridHeaderBorderColor = Color.FromArgb(218,218,222);
            this.GridHeaderHoverBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderHoverForeColor = Color.FromArgb(44,44,44);
            this.GridHeaderSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderSelectedForeColor = Color.FromArgb(44,44,44);
            this.GridHeaderHoverBorderColor = Color.FromArgb(218,218,222);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(218,218,222);
            this.GridRowHoverBackColor = Color.FromArgb(255,255,255);
            this.GridRowHoverForeColor = Color.FromArgb(44,44,44);
            this.GridRowSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridRowSelectedForeColor = Color.FromArgb(44,44,44);
            this.GridRowHoverBorderColor = Color.FromArgb(218,218,222);
            this.GridRowSelectedBorderColor = Color.FromArgb(218,218,222);
            this.GridLineColor = Color.FromArgb(225, 227, 230);
        }
    }
}