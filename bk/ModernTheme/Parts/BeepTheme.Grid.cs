using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(246,248,252);
            this.GridForeColor = Color.FromArgb(17,24,39);
            this.GridHeaderBackColor = Color.FromArgb(246,248,252);
            this.GridHeaderForeColor = Color.FromArgb(17,24,39);
            this.GridHeaderBorderColor = Color.FromArgb(203,213,225);
            this.GridHeaderHoverBackColor = Color.FromArgb(246,248,252);
            this.GridHeaderHoverForeColor = Color.FromArgb(17,24,39);
            this.GridHeaderSelectedBackColor = Color.FromArgb(246,248,252);
            this.GridHeaderSelectedForeColor = Color.FromArgb(17,24,39);
            this.GridHeaderHoverBorderColor = Color.FromArgb(203,213,225);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(203,213,225);
            this.GridRowHoverBackColor = Color.FromArgb(246,248,252);
            this.GridRowHoverForeColor = Color.FromArgb(17,24,39);
            this.GridRowSelectedBackColor = Color.FromArgb(246,248,252);
            this.GridRowSelectedForeColor = Color.FromArgb(17,24,39);
            this.GridRowHoverBorderColor = Color.FromArgb(203,213,225);
            this.GridRowSelectedBorderColor = Color.FromArgb(203,213,225);
            this.GridLineColor = Color.FromArgb(229,231,235);
        }
    }
}