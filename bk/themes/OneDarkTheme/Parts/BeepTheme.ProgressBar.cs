using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(40,44,52);
            this.ProgressBarForeColor = Color.FromArgb(171,178,191);
            this.ProgressBarBorderColor = Color.FromArgb(92,99,112);
            this.ProgressBarChunkColor = Color.FromArgb(40,44,52);
            this.ProgressBarErrorColor = Color.FromArgb(40,44,52);
            this.ProgressBarSuccessColor = Color.FromArgb(40,44,52);
            this.ProgressBarInsideTextColor = Color.FromArgb(171,178,191);
            this.ProgressBarHoverBackColor = Color.FromArgb(40,44,52);
            this.ProgressBarHoverForeColor = Color.FromArgb(171,178,191);
            this.ProgressBarHoverBorderColor = Color.FromArgb(92,99,112);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(171,178,191);
        }
    }
}