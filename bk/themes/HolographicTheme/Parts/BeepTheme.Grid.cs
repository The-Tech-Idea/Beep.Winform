using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(20,22,42);
            this.GridForeColor = Color.FromArgb(245,247,255);
            this.GridHeaderBackColor = Color.FromArgb(20,22,42);
            this.GridHeaderForeColor = Color.FromArgb(245,247,255);
            this.GridHeaderBorderColor = Color.FromArgb(74,79,123);
            this.GridHeaderHoverBackColor = Color.FromArgb(20,22,42);
            this.GridHeaderHoverForeColor = Color.FromArgb(245,247,255);
            this.GridHeaderSelectedBackColor = Color.FromArgb(20,22,42);
            this.GridHeaderSelectedForeColor = Color.FromArgb(245,247,255);
            this.GridHeaderHoverBorderColor = Color.FromArgb(74,79,123);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(74,79,123);
            this.GridRowHoverBackColor = Color.FromArgb(20,22,42);
            this.GridRowHoverForeColor = Color.FromArgb(245,247,255);
            this.GridRowSelectedBackColor = Color.FromArgb(20,22,42);
            this.GridRowSelectedForeColor = Color.FromArgb(245,247,255);
            this.GridRowHoverBorderColor = Color.FromArgb(74,79,123);
            this.GridRowSelectedBorderColor = Color.FromArgb(74,79,123);
            this.GridLineColor = Color.FromArgb(74,79,123);
        }
    }
}