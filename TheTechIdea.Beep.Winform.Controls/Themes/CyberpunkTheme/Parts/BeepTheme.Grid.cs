using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(18,16,34);
            this.GridForeColor = Color.FromArgb(228,244,255);
            this.GridHeaderBackColor = Color.FromArgb(18,16,34);
            this.GridHeaderForeColor = Color.FromArgb(228,244,255);
            this.GridHeaderBorderColor = Color.FromArgb(90,20,110);
            this.GridHeaderHoverBackColor = Color.FromArgb(18,16,34);
            this.GridHeaderHoverForeColor = Color.FromArgb(228,244,255);
            this.GridHeaderSelectedBackColor = Color.FromArgb(18,16,34);
            this.GridHeaderSelectedForeColor = Color.FromArgb(228,244,255);
            this.GridHeaderHoverBorderColor = Color.FromArgb(90,20,110);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(90,20,110);
            this.GridRowHoverBackColor = Color.FromArgb(18,16,34);
            this.GridRowHoverForeColor = Color.FromArgb(228,244,255);
            this.GridRowSelectedBackColor = Color.FromArgb(18,16,34);
            this.GridRowSelectedForeColor = Color.FromArgb(228,244,255);
            this.GridRowHoverBorderColor = Color.FromArgb(90,20,110);
            this.GridRowSelectedBorderColor = Color.FromArgb(90,20,110);
            this.GridLineColor = Color.FromArgb(90,20,110);
        }
    }
}