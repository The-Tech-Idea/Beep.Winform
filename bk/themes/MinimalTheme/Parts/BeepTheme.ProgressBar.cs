using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(255,255,255);
            this.ProgressBarForeColor = Color.FromArgb(31,41,55);
            this.ProgressBarBorderColor = Color.FromArgb(209,213,219);
            this.ProgressBarChunkColor = Color.FromArgb(255,255,255);
            this.ProgressBarErrorColor = Color.FromArgb(239,68,68);
            this.ProgressBarSuccessColor = Color.FromArgb(16,185,129);
            this.ProgressBarInsideTextColor = Color.FromArgb(31,41,55);
            this.ProgressBarHoverBackColor = Color.FromArgb(255,255,255);
            this.ProgressBarHoverForeColor = Color.FromArgb(31,41,55);
            this.ProgressBarHoverBorderColor = Color.FromArgb(209,213,219);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(31,41,55);
        }
    }
}