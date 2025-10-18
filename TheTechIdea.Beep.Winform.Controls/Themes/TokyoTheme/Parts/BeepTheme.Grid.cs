using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(32,35,50);
            this.GridForeColor = Color.FromArgb(192,202,245);
            this.GridHeaderBackColor = Color.FromArgb(32,35,50);
            this.GridHeaderForeColor = Color.FromArgb(192,202,245);
            this.GridHeaderBorderColor = Color.FromArgb(86,95,137);
            this.GridHeaderHoverBackColor = Color.FromArgb(32,35,50);
            this.GridHeaderHoverForeColor = Color.FromArgb(192,202,245);
            this.GridHeaderSelectedBackColor = Color.FromArgb(32,35,50);
            this.GridHeaderSelectedForeColor = Color.FromArgb(192,202,245);
            this.GridHeaderHoverBorderColor = Color.FromArgb(86,95,137);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(86,95,137);
            this.GridRowHoverBackColor = Color.FromArgb(32,35,50);
            this.GridRowHoverForeColor = Color.FromArgb(192,202,245);
            this.GridRowSelectedBackColor = Color.FromArgb(32,35,50);
            this.GridRowSelectedForeColor = Color.FromArgb(192,202,245);
            this.GridRowHoverBorderColor = Color.FromArgb(86,95,137);
            this.GridRowSelectedBorderColor = Color.FromArgb(86,95,137);
            this.GridLineColor = Color.FromArgb(86,95,137);
        }
    }
}