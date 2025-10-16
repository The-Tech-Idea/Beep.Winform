using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(255,255,255);
            this.GridForeColor = Color.FromArgb(46,52,54);
            this.GridHeaderBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderForeColor = Color.FromArgb(46,52,54);
            this.GridHeaderBorderColor = Color.FromArgb(205,207,212);
            this.GridHeaderHoverBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderHoverForeColor = Color.FromArgb(46,52,54);
            this.GridHeaderSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderSelectedForeColor = Color.FromArgb(46,52,54);
            this.GridHeaderHoverBorderColor = Color.FromArgb(205,207,212);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(205,207,212);
            this.GridRowHoverBackColor = Color.FromArgb(255,255,255);
            this.GridRowHoverForeColor = Color.FromArgb(46,52,54);
            this.GridRowSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridRowSelectedForeColor = Color.FromArgb(46,52,54);
            this.GridRowHoverBorderColor = Color.FromArgb(205,207,212);
            this.GridRowSelectedBorderColor = Color.FromArgb(205,207,212);
            this.GridLineColor = Color.FromArgb(225, 227, 230);
        }
    }
}