using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(236,244,255);
            this.ProgressBarForeColor = Color.FromArgb(17,24,39);
            this.ProgressBarBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ProgressBarChunkColor = Color.FromArgb(236,244,255);
            this.ProgressBarErrorColor = Color.FromArgb(239,68,68);
            this.ProgressBarSuccessColor = Color.FromArgb(16,185,129);
            this.ProgressBarInsideTextColor = Color.FromArgb(17,24,39);
            this.ProgressBarHoverBackColor = Color.FromArgb(236,244,255);
            this.ProgressBarHoverForeColor = Color.FromArgb(17,24,39);
            this.ProgressBarHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(17,24,39);
        }
    }
}