using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(50,52,64);
            this.GridForeColor = Color.FromArgb(248,248,242);
            this.GridHeaderBackColor = Color.FromArgb(50,52,64);
            this.GridHeaderForeColor = Color.FromArgb(248,248,242);
            this.GridHeaderBorderColor = Color.FromArgb(98,114,164);
            this.GridHeaderHoverBackColor = Color.FromArgb(50,52,64);
            this.GridHeaderHoverForeColor = Color.FromArgb(248,248,242);
            this.GridHeaderSelectedBackColor = Color.FromArgb(50,52,64);
            this.GridHeaderSelectedForeColor = Color.FromArgb(248,248,242);
            this.GridHeaderHoverBorderColor = Color.FromArgb(98,114,164);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(98,114,164);
            this.GridRowHoverBackColor = Color.FromArgb(50,52,64);
            this.GridRowHoverForeColor = Color.FromArgb(248,248,242);
            this.GridRowSelectedBackColor = Color.FromArgb(50,52,64);
            this.GridRowSelectedForeColor = Color.FromArgb(248,248,242);
            this.GridRowHoverBorderColor = Color.FromArgb(98,114,164);
            this.GridRowSelectedBorderColor = Color.FromArgb(98,114,164);
            this.GridLineColor = Color.FromArgb(98,114,164);
        }
    }
}