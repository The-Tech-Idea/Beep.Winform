using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(243,242,241);
            this.ProgressBarForeColor = Color.FromArgb(32,31,30);
            this.ProgressBarBorderColor = Color.FromArgb(220,220,220);
            this.ProgressBarChunkColor = Color.FromArgb(243,242,241);
            this.ProgressBarErrorColor = Color.FromArgb(232,17,35);
            this.ProgressBarSuccessColor = Color.FromArgb(0,153,51);
            this.ProgressBarInsideTextColor = Color.FromArgb(32,31,30);
            this.ProgressBarHoverBackColor = Color.FromArgb(243,242,241);
            this.ProgressBarHoverForeColor = Color.FromArgb(32,31,30);
            this.ProgressBarHoverBorderColor = Color.FromArgb(220,220,220);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(32,31,30);
        }
    }
}