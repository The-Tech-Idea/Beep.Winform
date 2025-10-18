using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(255,255,255);
            this.GridForeColor = Color.FromArgb(33,33,33);
            this.GridHeaderBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderForeColor = Color.FromArgb(33,33,33);
            this.GridHeaderBorderColor = Color.FromArgb(224,224,224);
            this.GridHeaderHoverBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderHoverForeColor = Color.FromArgb(33,33,33);
            this.GridHeaderSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderSelectedForeColor = Color.FromArgb(33,33,33);
            this.GridHeaderHoverBorderColor = Color.FromArgb(224,224,224);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(224,224,224);
            this.GridRowHoverBackColor = Color.FromArgb(255,255,255);
            this.GridRowHoverForeColor = Color.FromArgb(33,33,33);
            this.GridRowSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridRowSelectedForeColor = Color.FromArgb(33,33,33);
            this.GridRowHoverBorderColor = Color.FromArgb(224,224,224);
            this.GridRowSelectedBorderColor = Color.FromArgb(224,224,224);
            this.GridLineColor = Color.FromArgb(224,224,224);
        }
    }
}