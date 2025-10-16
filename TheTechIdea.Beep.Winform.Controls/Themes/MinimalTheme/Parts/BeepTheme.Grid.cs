using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(249,250,251);
            this.GridForeColor = Color.FromArgb(31,41,55);
            this.GridHeaderBackColor = Color.FromArgb(249,250,251);
            this.GridHeaderForeColor = Color.FromArgb(31,41,55);
            this.GridHeaderBorderColor = Color.FromArgb(209,213,219);
            this.GridHeaderHoverBackColor = Color.FromArgb(249,250,251);
            this.GridHeaderHoverForeColor = Color.FromArgb(31,41,55);
            this.GridHeaderSelectedBackColor = Color.FromArgb(249,250,251);
            this.GridHeaderSelectedForeColor = Color.FromArgb(31,41,55);
            this.GridHeaderHoverBorderColor = Color.FromArgb(209,213,219);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(209,213,219);
            this.GridRowHoverBackColor = Color.FromArgb(249,250,251);
            this.GridRowHoverForeColor = Color.FromArgb(31,41,55);
            this.GridRowSelectedBackColor = Color.FromArgb(249,250,251);
            this.GridRowSelectedForeColor = Color.FromArgb(31,41,55);
            this.GridRowHoverBorderColor = Color.FromArgb(209,213,219);
            this.GridRowSelectedBorderColor = Color.FromArgb(209,213,219);
            this.GridLineColor = Color.FromArgb(229,231,235);
        }
    }
}