using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(250,250,252);
            this.ProgressBarForeColor = Color.FromArgb(28,28,30);
            this.ProgressBarBorderColor = Color.FromArgb(229,229,234);
            this.ProgressBarChunkColor = Color.FromArgb(250,250,252);
            this.ProgressBarErrorColor = Color.FromArgb(255,69,58);
            this.ProgressBarSuccessColor = Color.FromArgb(48,209,88);
            this.ProgressBarInsideTextColor = Color.FromArgb(28,28,30);
            this.ProgressBarHoverBackColor = Color.FromArgb(250,250,252);
            this.ProgressBarHoverForeColor = Color.FromArgb(28,28,30);
            this.ProgressBarHoverBorderColor = Color.FromArgb(229,229,234);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(28,28,30);
        }
    }
}