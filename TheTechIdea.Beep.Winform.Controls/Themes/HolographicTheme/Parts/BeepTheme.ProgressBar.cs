using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(15,16,32);
            this.ProgressBarForeColor = Color.FromArgb(245,247,255);
            this.ProgressBarBorderColor = Color.FromArgb(74,79,123);
            this.ProgressBarChunkColor = Color.FromArgb(15,16,32);
            this.ProgressBarErrorColor = Color.FromArgb(15,16,32);
            this.ProgressBarSuccessColor = Color.FromArgb(15,16,32);
            this.ProgressBarInsideTextColor = Color.FromArgb(245,247,255);
            this.ProgressBarHoverBackColor = Color.FromArgb(15,16,32);
            this.ProgressBarHoverForeColor = Color.FromArgb(245,247,255);
            this.ProgressBarHoverBorderColor = Color.FromArgb(74,79,123);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(245,247,255);
        }
    }
}