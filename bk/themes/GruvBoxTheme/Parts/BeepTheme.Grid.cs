using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(45,42,40);
            this.GridForeColor = Color.FromArgb(235,219,178);
            this.GridHeaderBackColor = Color.FromArgb(45,42,40);
            this.GridHeaderForeColor = Color.FromArgb(235,219,178);
            this.GridHeaderBorderColor = Color.FromArgb(168,153,132);
            this.GridHeaderHoverBackColor = Color.FromArgb(45,42,40);
            this.GridHeaderHoverForeColor = Color.FromArgb(235,219,178);
            this.GridHeaderSelectedBackColor = Color.FromArgb(45,42,40);
            this.GridHeaderSelectedForeColor = Color.FromArgb(235,219,178);
            this.GridHeaderHoverBorderColor = Color.FromArgb(168,153,132);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(168,153,132);
            this.GridRowHoverBackColor = Color.FromArgb(45,42,40);
            this.GridRowHoverForeColor = Color.FromArgb(235,219,178);
            this.GridRowSelectedBackColor = Color.FromArgb(45,42,40);
            this.GridRowSelectedForeColor = Color.FromArgb(235,219,178);
            this.GridRowHoverBorderColor = Color.FromArgb(168,153,132);
            this.GridRowSelectedBorderColor = Color.FromArgb(168,153,132);
            this.GridLineColor = Color.FromArgb(168,153,132);
        }
    }
}