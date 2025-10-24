using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(245,246,247);
            this.ProgressBarForeColor = Color.FromArgb(43,45,48);
            this.ProgressBarBorderColor = Color.FromArgb(220,223,230);
            this.ProgressBarChunkColor = Color.FromArgb(245,246,247);
            this.ProgressBarErrorColor = Color.FromArgb(244,67,54);
            this.ProgressBarSuccessColor = Color.FromArgb(76,175,80);
            this.ProgressBarInsideTextColor = Color.FromArgb(43,45,48);
            this.ProgressBarHoverBackColor = Color.FromArgb(245,246,247);
            this.ProgressBarHoverForeColor = Color.FromArgb(43,45,48);
            this.ProgressBarHoverBorderColor = Color.FromArgb(220,223,230);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(43,45,48);
        }
    }
}