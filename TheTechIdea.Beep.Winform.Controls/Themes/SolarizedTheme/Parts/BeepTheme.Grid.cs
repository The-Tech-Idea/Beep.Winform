using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(12,44,56);
            this.GridForeColor = Color.FromArgb(147,161,161);
            this.GridHeaderBackColor = Color.FromArgb(12,44,56);
            this.GridHeaderForeColor = Color.FromArgb(147,161,161);
            this.GridHeaderBorderColor = Color.FromArgb(88,110,117);
            this.GridHeaderHoverBackColor = Color.FromArgb(12,44,56);
            this.GridHeaderHoverForeColor = Color.FromArgb(147,161,161);
            this.GridHeaderSelectedBackColor = Color.FromArgb(12,44,56);
            this.GridHeaderSelectedForeColor = Color.FromArgb(147,161,161);
            this.GridHeaderHoverBorderColor = Color.FromArgb(88,110,117);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(88,110,117);
            this.GridRowHoverBackColor = Color.FromArgb(12,44,56);
            this.GridRowHoverForeColor = Color.FromArgb(147,161,161);
            this.GridRowSelectedBackColor = Color.FromArgb(12,44,56);
            this.GridRowSelectedForeColor = Color.FromArgb(147,161,161);
            this.GridRowHoverBorderColor = Color.FromArgb(88,110,117);
            this.GridRowSelectedBorderColor = Color.FromArgb(88,110,117);
            this.GridLineColor = Color.FromArgb(88,110,117);
        }
    }
}