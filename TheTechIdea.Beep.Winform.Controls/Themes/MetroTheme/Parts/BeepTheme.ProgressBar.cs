using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(243,242,241);
            this.ProgressBarForeColor = Color.FromArgb(32,31,30);
            this.ProgressBarBorderColor = Color.FromArgb(225,225,225);
            this.ProgressBarChunkColor = Color.FromArgb(243,242,241);
            this.ProgressBarErrorColor = Color.FromArgb(196,30,58);
            this.ProgressBarSuccessColor = Color.FromArgb(16,124,16);
            this.ProgressBarInsideTextColor = Color.FromArgb(32,31,30);
            this.ProgressBarHoverBackColor = Color.FromArgb(243,242,241);
            this.ProgressBarHoverForeColor = Color.FromArgb(32,31,30);
            this.ProgressBarHoverBorderColor = Color.FromArgb(225,225,225);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(32,31,30);
        }
    }
}