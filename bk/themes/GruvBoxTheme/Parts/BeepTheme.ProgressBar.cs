using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(40,40,40);
            this.ProgressBarForeColor = Color.FromArgb(235,219,178);
            this.ProgressBarBorderColor = Color.FromArgb(168,153,132);
            this.ProgressBarChunkColor = Color.FromArgb(40,40,40);
            this.ProgressBarErrorColor = Color.FromArgb(40,40,40);
            this.ProgressBarSuccessColor = Color.FromArgb(40,40,40);
            this.ProgressBarInsideTextColor = Color.FromArgb(235,219,178);
            this.ProgressBarHoverBackColor = Color.FromArgb(40,40,40);
            this.ProgressBarHoverForeColor = Color.FromArgb(235,219,178);
            this.ProgressBarHoverBorderColor = Color.FromArgb(168,153,132);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(235,219,178);
        }
    }
}