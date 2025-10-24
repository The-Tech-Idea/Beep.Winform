using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(67,76,94);
            this.GridForeColor = Color.FromArgb(216,222,233);
            this.GridHeaderBackColor = Color.FromArgb(67,76,94);
            this.GridHeaderForeColor = Color.FromArgb(216,222,233);
            this.GridHeaderBorderColor = Color.FromArgb(76,86,106);
            this.GridHeaderHoverBackColor = Color.FromArgb(67,76,94);
            this.GridHeaderHoverForeColor = Color.FromArgb(216,222,233);
            this.GridHeaderSelectedBackColor = Color.FromArgb(67,76,94);
            this.GridHeaderSelectedForeColor = Color.FromArgb(216,222,233);
            this.GridHeaderHoverBorderColor = Color.FromArgb(76,86,106);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(76,86,106);
            this.GridRowHoverBackColor = Color.FromArgb(67,76,94);
            this.GridRowHoverForeColor = Color.FromArgb(216,222,233);
            this.GridRowSelectedBackColor = Color.FromArgb(67,76,94);
            this.GridRowSelectedForeColor = Color.FromArgb(216,222,233);
            this.GridRowHoverBorderColor = Color.FromArgb(76,86,106);
            this.GridRowSelectedBorderColor = Color.FromArgb(76,86,106);
            this.GridLineColor = Color.FromArgb(76,86,106);
        }
    }
}