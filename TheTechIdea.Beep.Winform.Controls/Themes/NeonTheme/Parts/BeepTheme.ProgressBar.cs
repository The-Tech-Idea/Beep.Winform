using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(10,12,20);
            this.ProgressBarForeColor = Color.FromArgb(235,245,255);
            this.ProgressBarBorderColor = Color.FromArgb(60,70,100);
            this.ProgressBarChunkColor = Color.FromArgb(10,12,20);
            this.ProgressBarErrorColor = Color.FromArgb(10,12,20);
            this.ProgressBarSuccessColor = Color.FromArgb(10,12,20);
            this.ProgressBarInsideTextColor = Color.FromArgb(235,245,255);
            this.ProgressBarHoverBackColor = Color.FromArgb(10,12,20);
            this.ProgressBarHoverForeColor = Color.FromArgb(235,245,255);
            this.ProgressBarHoverBorderColor = Color.FromArgb(60,70,100);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(235,245,255);
        }
    }
}