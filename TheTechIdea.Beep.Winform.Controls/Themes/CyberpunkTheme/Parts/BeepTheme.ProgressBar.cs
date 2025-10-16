using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(10,8,20);
            this.ProgressBarForeColor = Color.FromArgb(228,244,255);
            this.ProgressBarBorderColor = Color.FromArgb(90,20,110);
            this.ProgressBarChunkColor = Color.FromArgb(10,8,20);
            this.ProgressBarErrorColor = Color.FromArgb(10,8,20);
            this.ProgressBarSuccessColor = Color.FromArgb(10,8,20);
            this.ProgressBarInsideTextColor = Color.FromArgb(228,244,255);
            this.ProgressBarHoverBackColor = Color.FromArgb(10,8,20);
            this.ProgressBarHoverForeColor = Color.FromArgb(228,244,255);
            this.ProgressBarHoverBorderColor = Color.FromArgb(90,20,110);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(228,244,255);
        }
    }
}