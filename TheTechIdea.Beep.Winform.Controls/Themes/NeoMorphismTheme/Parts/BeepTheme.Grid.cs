using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.GridForeColor = Color.FromArgb(58,66,86);
            this.GridHeaderBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.GridHeaderForeColor = Color.FromArgb(58,66,86);
            this.GridHeaderBorderColor = Color.FromArgb(221,228,235);
            this.GridHeaderHoverBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.GridHeaderHoverForeColor = Color.FromArgb(58,66,86);
            this.GridHeaderSelectedBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.GridHeaderSelectedForeColor = Color.FromArgb(58,66,86);
            this.GridHeaderHoverBorderColor = Color.FromArgb(221,228,235);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(221,228,235);
            this.GridRowHoverBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.GridRowHoverForeColor = Color.FromArgb(58,66,86);
            this.GridRowSelectedBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.GridRowSelectedForeColor = Color.FromArgb(58,66,86);
            this.GridRowHoverBorderColor = Color.FromArgb(221,228,235);
            this.GridRowSelectedBorderColor = Color.FromArgb(221,228,235);
            this.GridLineColor = Color.FromArgb(225, 232, 238);
        }
    }
}