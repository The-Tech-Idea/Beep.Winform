using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(36,40,48);
            this.GridForeColor = Color.FromArgb(171,178,191);
            this.GridHeaderBackColor = Color.FromArgb(36,40,48);
            this.GridHeaderForeColor = Color.FromArgb(171,178,191);
            this.GridHeaderBorderColor = Color.FromArgb(92,99,112);
            this.GridHeaderHoverBackColor = Color.FromArgb(36,40,48);
            this.GridHeaderHoverForeColor = Color.FromArgb(171,178,191);
            this.GridHeaderSelectedBackColor = Color.FromArgb(36,40,48);
            this.GridHeaderSelectedForeColor = Color.FromArgb(171,178,191);
            this.GridHeaderHoverBorderColor = Color.FromArgb(92,99,112);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(92,99,112);
            this.GridRowHoverBackColor = Color.FromArgb(36,40,48);
            this.GridRowHoverForeColor = Color.FromArgb(171,178,191);
            this.GridRowSelectedBackColor = Color.FromArgb(36,40,48);
            this.GridRowSelectedForeColor = Color.FromArgb(171,178,191);
            this.GridRowHoverBorderColor = Color.FromArgb(92,99,112);
            this.GridRowSelectedBorderColor = Color.FromArgb(92,99,112);
            this.GridLineColor = Color.FromArgb(92,99,112);
        }
    }
}