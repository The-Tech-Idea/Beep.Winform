using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(245,246,248);
            this.ProgressBarForeColor = Color.FromArgb(32,32,32);
            this.ProgressBarBorderColor = Color.FromArgb(218,223,230);
            this.ProgressBarChunkColor = Color.FromArgb(245,246,248);
            this.ProgressBarErrorColor = Color.FromArgb(196,30,58);
            this.ProgressBarSuccessColor = Color.FromArgb(16,124,16);
            this.ProgressBarInsideTextColor = Color.FromArgb(32,32,32);
            this.ProgressBarHoverBackColor = Color.FromArgb(245,246,248);
            this.ProgressBarHoverForeColor = Color.FromArgb(32,32,32);
            this.ProgressBarHoverBorderColor = Color.FromArgb(218,223,230);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(32,32,32);
        }
    }
}